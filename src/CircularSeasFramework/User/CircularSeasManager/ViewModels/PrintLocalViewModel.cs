using System;
using System.Collections.Generic;
using System.Text;
using CircularSeasManager.Models;
using System.Threading.Tasks;
using CircularSeasManager.Services;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using CircularSeasManager.Resources;
namespace CircularSeasManager.ViewModels {
    class PrintLocalViewModel : PrintLocalModel {

        //DI
        public Services.OctoClient OctoClient => DependencyService.Get<Services.OctoClient>();
        public Services.SliceClient SliceClient => DependencyService.Get<Services.SliceClient>();
        public Command CmdFicherosLocales { get; set; }
        public Command CmdSendToPrint { get; set; }
        public Command CmdDelete { get; set; }

        public PrintLocalViewModel() {
            FilesCollection = new ObservableCollection<string>();
            CmdFicherosLocales = new Command(async () => await GetlocalFiles());
            CmdSendToPrint = new Command(async () => await SendToPrint());
            CmdDelete = new Command(async () => await SendToDelete());
            //La llamada no es awaited porque el constructor no es async, simplemente se ejecuta más tarde pero copia iguamente en la lista.
            _ = GetlocalFiles();
        }

        
        public async Task GetlocalFiles() {
            
            var resp = await OctoClient.GetFiles();
            if (OctoClient.ResultRequest == RequestState.Ok) {
                //Copia la lista de ficheros que se devuelve en la colección, para que por binding se muestre en el listview
                resp.ForEach(x => FilesCollection.Add(x));
            }
            else {
                if (OctoClient.ResultRequest == RequestState.NoConnection) {
                    await AlertConnectionLost();
                }
            }
        }

        public async Task SendToPrint() {
            if (SelectedGCODE == null) {
                //Tratamiento, no se seleccionó ningún gcode para imprimir.
                await Application.Current.MainPage.DisplayAlert(AlertResources.PrintingHeaderError,
                            AlertResources.PrintingBodySelectOne,
                            AlertResources.PrintingReturn);
            }
            else {
                var estado = await OctoClient.PostPrintFile(SelectedGCODE);
                if (estado == false) { //si no se pudo hacer, se comprueba por qué
                    if (OctoClient.ResultRequest == RequestState.NotExist) {
                        //Tratamiento de que no existe ese fichero
                    }
                    else if (OctoClient.ResultRequest == RequestState.Busy) {
                        //Tratamiento de que la impresora se encuentra imprimiendo
                        await Application.Current.MainPage.DisplayAlert(AlertResources.PrintingHeaderError,
                            AlertResources.PrintingBodyProcessing,
                            AlertResources.PrintingReturn);
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                    else if (OctoClient.ResultRequest == RequestState.FailPrinterConnection) {
                        //No se pudo establecer conexión con la impresora
                    }
                }
                else { //todo va bien
                    //Retrocede a la página anterior, si se hizo de forma correcta.
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
            }
        }

        public async Task SendToDelete() {
            if (SelectedGCODE == null) {
                //Tratamiento, no se seleccionó ningún gcode para eliminar
                await Application.Current.MainPage.DisplayAlert(AlertResources.DeletingHeader,
                            AlertResources.DeletingBodySelectOne,
                            AlertResources.PrintingReturn);
            }
            else {
                var estado = await OctoClient.DeleteFile(SelectedGCODE);
                FilesCollection.Remove(SelectedGCODE);
                if (estado == false) {
                   //Cuando hubo error en la operación
                    if (OctoClient.ResultRequest == RequestState.NotExist) {
                        //Tratamiento de que no existe ese fichero
                    }
                    if (OctoClient.ResultRequest == RequestState.Busy) {
                        //Tratamiento de que ese fichero está siendo impreso y por lo tanto no se puede eliminar
                        await Application.Current.MainPage.DisplayAlert(AlertResources.DeletingHeader,
                            AlertResources.DeletingBodyErrorProcesing,
                            AlertResources.PrintingReturn);
                    }
                }
                else {
                    
                }
            }
        }
    }
}
