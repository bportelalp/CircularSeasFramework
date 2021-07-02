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

        //Definición de los comandos
        public Command CmdCerrarSesion { get; set; }
        public Command CmdImprimirLocal { get; set; }
        public Command CmdDetener { get; set; }
        public Command CmdPausar { get; set; }
        public Command CmdSubirGCODE { get; set; }
        public Command CmdSlice { get; set; }
        public Command CmdConectar { get; set; }

        //Constructor
        public MainPanelViewModel() {
            //Inicia temporizador
            InPage = true;
            Device.StartTimer(TimeSpan.FromSeconds(1.5), OnTimerTick);
            CmdCerrarSesion = new Command(async () => await CerrarSesion(),()=>!Busy);
            CmdImprimirLocal = new Command(async () => await ImprimirLocal(),()=>!Busy);
            CmdDetener = new Command(async () => await Detener(),()=>!Busy);
            CmdPausar = new Command(async () => await Pausar(),()=>!Busy);
            CmdSubirGCODE = new Command(async() => await SubirGCODE(), () => !Busy);
            CmdSlice = new Command(async () => await AbrirSlicePage(), () => !Busy);
            CmdConectar = new Command(async () => await ConectarImpresora(), () => !Busy);
            
        }

        bool OnTimerTick() {
            if (InPage) {
                Device.BeginInvokeOnMainThread(async () => {
                    await VisualizacionDatos();
                });
                return true;
            }
            else { return false; }
        }

        private async Task CerrarSesion() {
            Busy = true;
            var resultado = await Global.PrinterClient.Logout();
            Busy = false;
            if (resultado) {
                Application.Current.MainPage = new NavigationPage(new Views.LoginPage());
                InPage = false;
            }
        }

        private async Task ImprimirLocal() {
            //Añade nueva página en la pila de navegación con la pestaña para imprimir
            await Application.Current.MainPage.Navigation.PushAsync(new Views.PrintLocal());
            
        }

        private async Task VisualizacionDatos() {
            //Obtener datos de trabajo actual
            var trabajo = await Models.Global.PrinterClient.GetCurrentjob();
            if (trabajo != null) {
                PrinterState = trabajo.state;
                FileName = trabajo.job.file.name;

                //Actualiza botón.
                if (PrinterState == "Pausing" | PrinterState == "Paused") {
                    PauseOrResume = AppResources.btnPause;
                }
                else { PauseOrResume = AppResources.btnResume; }

                //Actualiza tiempo de trabajo
                if (trabajo.progress.printTimeLeft != null) {
                    PrintTimeLeft = TimeSpan.FromSeconds((double)trabajo.progress.printTimeLeft);
                }
                else {
                    PrintTimeLeft = TimeSpan.FromSeconds(0);
                }

                //Actualiza progreso
                if (trabajo.progress.completion != null)
                    Progress = (float)(trabajo.progress.completion);
                else {
                    Progress = 0;
                }
                
            }
            else {
                if (Global.PrinterClient.ResultRequest == RequestState.NoConnection) {
                    //Implementación de espera a conexión
                }
            }

            Services.PrinterStateJSON.RootObj printer = await Models.Global.PrinterClient.GetPrinterState();
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
            else if (Global.PrinterClient.ResultRequest == RequestState.Other) {
                /*Si pasa esto, es debido a Conflict de "Printer is not operational", así que 
                 se ponen a 0*/
                HotendTemp = 0;
                BedTemp = 0;
                PrinterOffline = true;
            }

        }

        private async Task Detener() {
            Busy = true;
            var estado = await Global.PrinterClient.PostJobCommand("cancel");
            Busy = false;
            if (!estado) {
                if (Global.PrinterClient.ResultRequest == RequestState.NoConnection) {
                    await AvisoPerdidaConexion();
                }
                
            }
        }

        private async Task Pausar() {
            Busy = true;
            var estado = await Global.PrinterClient.PostJobCommand("pause");
            Busy = false;
            if (!estado) {
                if (Global.PrinterClient.ResultRequest == RequestState.NoConnection) {
                    await AvisoPerdidaConexion();
                }

            }
        }

        private async Task SubirGCODE() {

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

                    var estado = await Global.PrinterClient.UploadFile(gco.DataArray, gco.FileName, quiereimprimir);
                    Busy = false;
                    if (!estado) {
                        if (Global.PrinterClient.ResultRequest == RequestState.NoConnection) {
                            await AvisoPerdidaConexion();
                        }
                        if (Global.PrinterClient.ResultRequest == RequestState.BadFileExtension) {
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

        private async Task AbrirSlicePage() {
            //Abrir página de Slicer
            InPage = false;
            await Application.Current.MainPage.Navigation.PushAsync(new Views.SlicerPage());
            InPage = true;
            Device.StartTimer(TimeSpan.FromSeconds(1.5), OnTimerTick);
        }

        private async Task ConectarImpresora() {
            Busy = true;
            var resultado = await Global.PrinterClient.PostConexPrinter(true, "/dev/ttyACM0", 250000, "_default");
            Busy = false;
        }
    }
}
