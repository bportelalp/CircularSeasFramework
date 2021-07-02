using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using CircularSeasManager.Models;

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
        public Command CmdEnviarSTL { get; set; }
        public Command CmdAyuda { get; set; }
        public Command CmdPickSTL { get; set; }

        //Constructor
        public SliceViewModel() {
            CmdEnviarSTL = new Command(async () => await EnviarSTL());
            CmdAyuda = new Command(async () => await AbrirAsistente(), () => !Ocupado);
            CmdPickSTL = new Command(async () => await PickSTL());


            //Inicializar las colecciones
            MaterialCollection = new ObservableCollection<string>();
            CalidadCollection = new ObservableCollection<string>();

            //llamada para rellenarlos campos
            _ = ObtenerDatos();
            /*
            MaterialCollection.Add("PLA");
            MaterialCollection.Add("ABS");
            MaterialCollection.Add("Recycled #1"); 
            MaterialCollection.Add("Recycled #2");
            MaterialCollection.Add("PETG");
            MaterialCollection.Add("HIPS");
            MaterialCollection.Add("Nylon");
            MaterialCollection.Add("Recycled #3");
            MaterialCollection.Add("Recycled #4");
            CalidadCollection.Add("High quality");
            CalidadCollection.Add("Standard");
            CalidadCollection.Add("Extra strength");
            CalidadCollection.Add("Fast prototyping");*/
        }

        public async Task ObtenerDatos() {

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

            DataMaterial = await Global.ClienteSlice.GetDatos(printer);
            if (DataMaterial != null) {
                //Cargar a información nas coleccións para visualizar
                foreach (CircularSeas.Models.Filament item in DataMaterial.Filaments) {
                    MaterialCollection.Add(item.Name);
                }
                foreach (string item in DataMaterial.Printer.Profiles) {
                    CalidadCollection.Add(item);
                }
            }
            else {
                if (Global.ClienteSlice.resultRequest == HttpStatusCode.NotFound) {

                }
                else if (Global.ClienteSlice.resultRequest == 0) {
                    //Sin conexión
                    await Application.Current.MainPage.DisplayAlert("Erro", "Non se puido contactar co servidor. Inténteo de novo máis tarde", "Volver");
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
            }

        }

        public async Task PickSTL() {
            Ocupado = true;
            STL = await CrossFilePicker.Current.PickFile(new string[] { ".stl", ".STL" });
            try {
                if (!STL.FileName.EndsWith(".stl") && !STL.FileName.EndsWith(".STL")) {
                    await Application.Current.MainPage.DisplayAlert("Error", "Debes seleccionar sólo ficheros stl", "ok");
                    STL = null;
                }


            }
            catch {
                await Application.Current.MainPage.DisplayAlert("Erro", "Non seleccionou ningún arquivo ", "Volver");
            }
            Ocupado = false;
        }

        public async Task EnviarSTL() {
            //Implementación para enviar el stl e recibilo convertido
            Ocupado = true;
            if (TodoListo) {
                Tuple<string, byte[]> datos = new Tuple<string, byte[]>(null, null);
                MensajeStatus = "Laminando " + STL.FileName;
                datos = await Global.ClienteSlice.PostSTL(STL, printer, materialSelected, calidadSelected, usarSoporte);
                if (Global.ClienteSlice.resultRequest == HttpStatusCode.OK) {
                    MensajeStatus = "Subiendo a servicio local";
                    await Global.ClientePrint.UploadFile(datos.Item2, datos.Item1, false);
                    MensajeStatus = "Completado";
                    await Application.Current.MainPage.DisplayAlert("Listo", "Arquivo dispoñible para a impresión dende ficheiros " +
                        "locales", "Aceptar");
                }
                else {
                    if (Global.ClienteSlice.resultRequest == HttpStatusCode.PreconditionFailed) {
                        await Application.Current.MainPage.DisplayAlert("Erro", "Non se puido completar a solicitude. Probe a habilitar" +
                            "o soporte e se o problema persiste, contacte cun administrador", "Recibido");
                    }
                    if (Global.ClienteSlice.resultRequest == 0) {
                        await Application.Current.MainPage.DisplayAlert("Erro", "Erro de conexión", "Volver");
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                    if (Global.ClienteSlice.resultRequest == HttpStatusCode.BadRequest) {
                        await Application.Current.MainPage.DisplayAlert("Erro", "Fallo descoñecido", "Aceptar");
                    }
                }
            }
            else {
                await Application.Current.MainPage.DisplayAlert("Erro", "Debes seleccionar todolos parámetros", "Voltar");
            }
            Ocupado = false;
        }

        public async Task AbrirAsistente() {
            //Abrir asistente, se le pasa la información de los materiales.
            await Application.Current.MainPage.Navigation.PushAsync(new Views.AsistenteMaterial(DataMaterial));
        }

    }
}

