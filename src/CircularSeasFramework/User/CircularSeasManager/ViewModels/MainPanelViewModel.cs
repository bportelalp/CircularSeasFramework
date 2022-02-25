using System;
using System.Collections.Generic;
using System.Text;
using CircularSeasManager.Models;
using Xamarin.Forms;
using CircularSeasManager.Services;
using System.Threading.Tasks;
using Plugin.FilePicker;
using CircularSeasManager.Resources;

namespace CircularSeasManager.ViewModels {
    class MainPanelViewModel : MainPanelModel {

        //DI
        public Services.OctoClient OctoClient => DependencyService.Get<Services.OctoClient>();
        public Services.SliceClient SliceClient => DependencyService.Get<Services.SliceClient>();
        
        //Definición de los comandos
        public Command CmdLogout { get; set; }
        public Command CmdPrintLocal { get; set; }
        public Command CmdStop { get; set; }
        public Command CmdPause { get; set; }
        public Command CmdUploadGCODE { get; set; }
        public Command CmdSlice { get; set; }
        public Command CmdConnect { get; set; }
        public Command CmdOrder { get; set; }


        //Constructor
        public MainPanelViewModel() {
            //Inicia temporizador
            InPage = true;
            Device.StartTimer(TimeSpan.FromSeconds(1.5), OnTimerTick);
            CmdLogout = new Command(async () => await Logout(),()=>!Busy);
            CmdPrintLocal = new Command(async () => await PrintLocal(),()=>!Busy);
            CmdStop = new Command(async () => await StopPrinting(),()=>!Busy);
            CmdPause = new Command(async () => await PausePrinting(),()=>!Busy);
            CmdUploadGCODE = new Command(async() => await UploadGCODE(), () => !Busy);
            CmdSlice = new Command(async () => await OpenSlicePage(), () => !Busy);
            CmdConnect = new Command(async () => await ConnectPrinter(), () => !Busy);
            CmdOrder = new Command(async () => await OpenOrderPage(), () => !Busy);

        }

        bool OnTimerTick() {
            if (InPage) {
                Device.BeginInvokeOnMainThread(async () => {
                    await DataRefresh();
                });
                return true;
            }
            else { return false; }
        }

        private async Task Logout() {
            Busy = true;
            var result = await OctoClient.Logout();
            Busy = false;
            if (result) {
                Application.Current.MainPage = new NavigationPage(new Views.LoginPage());
                InPage = false;
            }
        }

        private async Task PrintLocal() {
            //Añade nueva página en la pila de navegación con la pestaña para imprimir
            await Application.Current.MainPage.Navigation.PushAsync(new Views.PrintLocal());
            
        }

        private async Task DataRefresh() {
            //Obtener datos de trabajo actual
            var currentJob = await OctoClient.GetCurrentjob();
            if (currentJob != null) {
                PrinterState = currentJob.state;
                FileName = currentJob.job.file.name;

                //Actualiza botón.
                if (PrinterState == "Pausing" | PrinterState == "Paused") {
                    PauseOrResume = AppResources.btnPause;
                }
                else { PauseOrResume = AppResources.btnResume; }

                //Actualiza tiempo de trabajo
                if (currentJob.progress.printTimeLeft != null) {
                    PrintTimeLeft = TimeSpan.FromSeconds((double)currentJob.progress.printTimeLeft);
                }
                else {
                    PrintTimeLeft = TimeSpan.FromSeconds(0);
                }

                //Actualiza progreso
                if (currentJob.progress.completion != null)
                    Progress = (float)(currentJob.progress.completion);
                else {
                    Progress = 0;
                }
                
            }
            else {
                if (OctoClient.ResultRequest == RequestState.NoConnection) {
                    //Coming to starpage
                    await AlertConnectionLost();
                    Application.Current.MainPage = new NavigationPage(new Views.LoginPage());

                }
            }

            Services.PrinterStateJSON.RootObj printer = await OctoClient.GetPrinterState();
            if (printer != null) {
                /*Puede haber un pequeño transitorio mientras conecta y octoprint le pide la info a la impresora, donde
                 tool0 y bed devuelve null, entonces no se puede acceder a actual y target*/
                PrinterOffline = false;
                try {
                    HotendTemp = printer.temperature.tool0.actual;
                    BedTemp = printer.temperature.bed.actual;
                }
                catch (Exception ex){
                    var error = ex;
                    //Ignora expeción, simplemente espera a que llegue el dato bueno
                }
            }
            else if (OctoClient.ResultRequest == RequestState.Other) {
                /*Si pasa esto, es debido a Conflict de "Printer is not operational", así que 
                 se ponen a 0*/
                HotendTemp = 0;
                BedTemp = 0;
                PrinterOffline = true;
            }

        }

        private async Task StopPrinting() {
            Busy = true;
            var estado = await OctoClient.PostJobCommand("cancel");
            Busy = false;
            if (!estado) {
                if (OctoClient.ResultRequest == RequestState.NoConnection) {
                    await AlertConnectionLost();
                }
                
            }
        }

        private async Task PausePrinting() {
            Busy = true;
            var estado = await OctoClient.PostJobCommand("pause");
            Busy = false;
            if (!estado) {
                if (OctoClient.ResultRequest == RequestState.NoConnection) {
                    await AlertConnectionLost();
                }

            }
        }

        private async Task UploadGCODE() {

            Busy = true;
            var gco = await CrossFilePicker.Current.PickFile(new string[] { ".gcode" });
            
            //Comprueba que efectivamente es un .gcode, pues en Android non se puede hacer filtro con .gcode
            try {
                if (gco.FileName.EndsWith(".gcode")) {
                    //Pregunta si se quiere imprimir directamente
                    bool quiereimprimir = false;
                    if (PrinterState == "Operational") {
                        quiereimprimir = await Application.Current.MainPage.DisplayAlert(AlertResources.WarningHeader,
                            AlertResources.PrintingDirectly,
                            AlertResources.Yes,
                            AlertResources.No);
                    }
                    else {
                        await Application.Current.MainPage.DisplayAlert(AlertResources.WarningHeader,
                            AlertResources.PrintingWorking,
                            AlertResources.Accept);
                    }

                    var estado = await OctoClient.UploadFile(gco.DataArray, gco.FileName, quiereimprimir);
                    Busy = false;
                    if (!estado) {
                        if (OctoClient.ResultRequest == RequestState.NoConnection) {
                            await AlertConnectionLost();
                        }
                        if (OctoClient.ResultRequest == RequestState.BadFileExtension) {
                            await Application.Current.MainPage.DisplayAlert(AlertResources.Error,
                                AlertResources.UploadOnlyGCODE,
                                AlertResources.Accept);
                        }
                    }
                    else {
                        //Notifica
                        await Application.Current.MainPage.DisplayAlert(AlertResources.Success,
                            AlertResources.SucessUpload,
                            AlertResources.Accept);
                    }
                }
                else {
                    //Si no es un gcode, se lanza advertencia
                    await Application.Current.MainPage.DisplayAlert(AlertResources.Error,
                                AlertResources.UploadOnlyGCODE,
                                AlertResources.Accept);
                }
            }
            catch (Exception NullReferenceException){
                await Application.Current.MainPage.DisplayAlert(AlertResources.Error,
                    AlertResources.FileNotProvided,
                    AlertResources.Accept);
            }
            
            Busy = false;

        }

        private async Task OpenSlicePage() {
            //Abrir página de Slicer
            InPage = false;
            await Application.Current.MainPage.Navigation.PushAsync(new Views.SlicerPage());
            InPage = true;
            Device.StartTimer(TimeSpan.FromSeconds(1.5), OnTimerTick);
        }

        private async Task OpenOrderPage()
        {
            //Abrir página de Slicer
            InPage = false;
            await Application.Current.MainPage.Navigation.PushAsync(new Views.OrderPage());
            InPage = true;
            Device.StartTimer(TimeSpan.FromSeconds(1.5), OnTimerTick);
        }

        private async Task ConnectPrinter() {
            Busy = true;
            var resultado = await OctoClient.PostConexPrinter(true, "/dev/ttyACM0", 250000, "_default");
            Busy = false;
        }
    }
}
