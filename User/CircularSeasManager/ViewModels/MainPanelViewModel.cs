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
            CmdCerrarSesion = new Command(async () => await CerrarSesion(),()=>!Ocupado);
            CmdImprimirLocal = new Command(async () => await ImprimirLocal(),()=>!Ocupado);
            CmdDetener = new Command(async () => await Detener(),()=>!Ocupado);
            CmdPausar = new Command(async () => await Pausar(),()=>!Ocupado);
            CmdSubirGCODE = new Command(async() => await SubirGCODE(), () => !Ocupado);
            CmdSlice = new Command(async () => await AbrirSlicePage(), () => !Ocupado);
            CmdConectar = new Command(async () => await ConectarImpresora(), () => !Ocupado);
            
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
            Ocupado = true;
            var resultado = await Global.ClientePrint.logout();
            Ocupado = false;
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
            var trabajo = await Models.Global.ClientePrint.GetCurrentjob();
            if (trabajo != null) {
                estadoImpresora = trabajo.state;
                nombreFichero = trabajo.job.file.name;

                //Actualiza botón.
                if (estadoImpresora == "Pausing" | estadoImpresora == "Paused") {
                    PausaResume = AppResources.btnPause;
                }
                else { PausaResume = AppResources.btnResume; }

                //Actualiza tiempo de trabajo
                if (trabajo.progress.printTimeLeft != null) {
                    PrintTimeLeft = TimeSpan.FromSeconds((double)trabajo.progress.printTimeLeft);
                }
                else {
                    PrintTimeLeft = TimeSpan.FromSeconds(0);
                }

                //Actualiza progreso
                if (trabajo.progress.completion != null)
                    progreso = (float)(trabajo.progress.completion);
                else {
                    progreso = 0;
                }
                
            }
            else {
                if (Global.ClientePrint.ResultRequest == EstadoRequest.SinConexion) {
                    //Implementación de espera a conexión
                }
            }

            Services.PrinterStateJSON.RootObj printer = await Models.Global.ClientePrint.GetPrinterState();
            if (printer != null) {
                /*Puede haber un pequeño transitorio mientras conecta y octoprint le pide la info a la impresora, donde
                 tool0 y bed devuelve null, entonces no se puede acceder a actual y target*/
                ImpresoraOffline = false;
                try {
                    HotendTemp = printer.temperature.tool0.actual;
                    BedTemp = printer.temperature.bed.actual;
                }
                catch (Exception ex){
                    var error = ex;
                    //Ignora expeción, simplemente espera a que llegue el dato bueno
                }
            }
            else if (Global.ClientePrint.ResultRequest == EstadoRequest.Otro) {
                /*Si pasa esto, es debido a Conflict de "Printer is not operational", así que 
                 se ponen a 0*/
                HotendTemp = 0;
                BedTemp = 0;
                ImpresoraOffline = true;
            }

        }

        private async Task Detener() {
            Ocupado = true;
            var estado = await Global.ClientePrint.PostComandoJob("cancel");
            Ocupado = false;
            if (!estado) {
                if (Global.ClientePrint.ResultRequest == EstadoRequest.SinConexion) {
                    await AvisoPerdidaConexion();
                }
                
            }
        }

        private async Task Pausar() {
            Ocupado = true;
            var estado = await Global.ClientePrint.PostComandoJob("pause");
            Ocupado = false;
            if (!estado) {
                if (Global.ClientePrint.ResultRequest == EstadoRequest.SinConexion) {
                    await AvisoPerdidaConexion();
                }

            }
        }

        private async Task SubirGCODE() {

            Ocupado = true;
            var gco = await CrossFilePicker.Current.PickFile(new string[] { ".gcode" });
            
            //Comprueba que efectivamente es un .gcode, pues en Android non se puede hacer filtro con .gcode
            try {
                if (gco.FileName.EndsWith(".gcode")) {
                    //Pregunta si se quiere imprimir directamente
                    bool quiereimprimir = false;
                    if (estadoImpresora == "Operational") {
                        quiereimprimir = await Application.Current.MainPage.DisplayAlert("Aviso", "Desea imprimir directamente el archivo seleccionado", "Si", "No");
                    }
                    else {
                        await Application.Current.MainPage.DisplayAlert("Aviso", "La impresora está trabajando. Se subirá el archivo pero" +
                            "debe esperar a que termine el actual. Posteriormente podrá seleccionarlo desde el menú imprimir local", "Entendido");
                    }

                    var estado = await Global.ClientePrint.UploadFile(gco.DataArray, gco.FileName, quiereimprimir);
                    Ocupado = false;
                    if (!estado) {
                        if (Global.ClientePrint.ResultRequest == EstadoRequest.SinConexion) {
                            await AvisoPerdidaConexion();
                        }
                        if (Global.ClientePrint.ResultRequest == EstadoRequest.ExtensionIncorrecta) {
                            await Application.Current.MainPage.DisplayAlert("Error", "Sólo se admiten ficheros con formato .gcode. No se pudo" +
                                " realizar la subida", "Entendido");
                        }
                    }
                    else {
                        //Notifica
                        await Application.Current.MainPage.DisplayAlert("Resultado", "Subida correcta", "Aceptar");
                    }
                }
                else {
                    //Si no es un gcode, se lanza advertencia
                    await Application.Current.MainPage.DisplayAlert("Error", "Sólo se admiten ficheros con formato .gcode. No se pudo" +
                                " realizar la subida", "Entendido");
                }
            }
            catch (Exception NullReferenceException){
                await Application.Current.MainPage.DisplayAlert("Error", "No se seleccionó ningún archivo", "Aceptar");
            }
            
            Ocupado = false;

        }

        private async Task AbrirSlicePage() {
            //Abrir página de Slicer
            InPage = false;
            await Application.Current.MainPage.Navigation.PushAsync(new Views.SlicerPage());
            InPage = true;
            Device.StartTimer(TimeSpan.FromSeconds(1.5), OnTimerTick);
        }

        private async Task ConectarImpresora() {
            Ocupado = true;
            var resultado = await Global.ClientePrint.PostConexPrinter(true, "/dev/ttyACM0", 250000, "_default");
            Ocupado = false;
        }
    }
}
