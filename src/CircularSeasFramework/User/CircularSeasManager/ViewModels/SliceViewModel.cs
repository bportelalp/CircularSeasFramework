using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using CircularSeasManager.Models;
using CircularSeasManager.Resources;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using Plugin.FilePicker;

namespace CircularSeasManager.ViewModels {
    public class SliceViewModel : SliceModel {

        //Comandos
        public Command CmdSendSTL { get; set; }
        public Command CmdHelp { get; set; }
        public Command CmdPickSTL { get; set; }

        //Constructor
        public SliceViewModel() {
            CmdSendSTL = new Command(async () => await SendSTL());
            CmdHelp = new Command(async () => await OpenAssistant(), () => !Busy);
            CmdPickSTL = new Command(async () => await PickSTL());

            //Inicializar las colecciones
            MaterialCollection = new ObservableCollection<string>();
            ProfileCollection = new ObservableCollection<string>();

            //llamada para rellenarlos campos
            _ = GetDataFromCloud();
        }

        public async Task GetDataFromCloud() {

            //Implementación GET para obtener datos de material y calidades para la impresora dada

            /*PROVISIONAL. Importar .json de datos como recurso embebido. En esta petición idealmente
            debería pedir el JSON al servicio REST con el que poder operar. En el vendría contenida toda
            la información del sistema. Quizá auí también pedir a octoprint la impresora utilizada para no
            estar cargándola todo el tiempo en la etapa anterior.*/
            /*
            var archivos = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            
            Stream original = Assembly.GetExecutingAssembly().GetManifestResourceStream("EntornoIntegrado.Modelo.Materiales.json");
            string reader = new StreamReader(original, Encoding.GetEncoding("iso-8859-1")).ReadToEnd();
            //Deserializar
            DataMaterial = JsonConvert.DeserializeObject<InfoTopsis>(reader);*/

            DataMaterial = await Global.ClienteSlice.GetData(printer);
            if (DataMaterial != null) {
                //Cargar a información nas coleccións para visualizar
                foreach (CircularSeas.Models.Filament item in DataMaterial.Filaments) {
                    MaterialCollection.Add(item.Name);
                }
                foreach (string item in DataMaterial.Printer.Profiles) {
                    ProfileCollection.Add(item);
                }
            }
            else {
                if (Global.ClienteSlice.resultRequest == HttpStatusCode.NotFound) {

                }
                else if (Global.ClienteSlice.resultRequest == 0) {
                    //Sin conexión
                    await Application.Current.MainPage.DisplayAlert(AlertResources.Error,
                        AlertResources.ServerDisconnected,
                        AlertResources.Accept);
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
            }

        }

        public async Task PickSTL() {
            Busy = true;
            STL = await CrossFilePicker.Current.PickFile(new string[] { ".stl", ".STL" });
            try {
                if (!STL.FileName.EndsWith(".stl") && !STL.FileName.EndsWith(".STL")) {
                    await Application.Current.MainPage.DisplayAlert(AlertResources.Error,
                        AlertResources.UploadOnlySTL,
                        AlertResources.Accept);
                    STL = null;
                }


            }
            catch {
                await Application.Current.MainPage.DisplayAlert(AlertResources.Error,
                        AlertResources.FileNotProvided,
                        AlertResources.Accept);
            }
            Busy = false;
        }

        public async Task SendSTL() {
            //Implementación para enviar el stl e recibilo convertido
            Busy = true;
            if (AllReady) {
                Tuple<string, byte[]> datos = new Tuple<string, byte[]>(null, null);
                StatusMessage = StringResources.Slicing + " " + STL.FileName;
                datos = await Global.ClienteSlice.PostSTL(STL, printer, MaterialSelected, ProfileSelected, UseSupport);
                if (Global.ClienteSlice.resultRequest == HttpStatusCode.OK) {
                    StatusMessage = StringResources.Uploading;
                    await Global.PrinterClient.UploadFile(datos.Item2, datos.Item1, false);
                    StatusMessage = StringResources.Completed;
                    await Application.Current.MainPage.DisplayAlert(AlertResources.Ready,
                        AlertResources.CanPrintedFromLocal,
                        AlertResources.Accept);
                }
                else {
                    if (Global.ClienteSlice.resultRequest == HttpStatusCode.PreconditionFailed) {
                        await Application.Current.MainPage.DisplayAlert(AlertResources.Error,
                        AlertResources.PerhapsSupportNeeded,
                        AlertResources.Accept);
                    }
                    if (Global.ClienteSlice.resultRequest == 0) {
                        await Application.Current.MainPage.DisplayAlert(AlertResources.Error,
                        AlertResources.ConnectionError,
                        AlertResources.Accept);
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                    if (Global.ClienteSlice.resultRequest == HttpStatusCode.BadRequest) {
                        await Application.Current.MainPage.DisplayAlert(AlertResources.Error,
                        AlertResources.UnknownError,
                        AlertResources.Accept);
                    }
                }
            }
            else {
                await Application.Current.MainPage.DisplayAlert(AlertResources.Error,
                        AlertResources.AllParametersMustProvide,
                        AlertResources.Accept);
            }
            Busy = false;
        }

        public async Task OpenAssistant() {
            //Abrir asistente, se le pasa la información de los materiales.
            await Application.Current.MainPage.Navigation.PushAsync(new Views.MaterialAssistantPage(DataMaterial));
        }

    }
}

