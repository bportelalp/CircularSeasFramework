﻿using System;
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

        public Command CmdFicherosLocales { get; set; }
        public Command CmdEnviarImprimir { get; set; }
        public Command CmdEliminar { get; set; }

        public PrintLocalViewModel() {
            ficherosCollection = new ObservableCollection<string>();
            CmdFicherosLocales = new Command(async () => await ObtenerArchivos());
            CmdEnviarImprimir = new Command(async () => await EnviarImprimir());
            CmdEliminar = new Command(async () => await EliminarFichero());
            //La llamada no es awaited porque el constructor no es async, simplemente se ejecuta más tarde pero copia iguamente en la lista.
            _ = ObtenerArchivos();
        }

        
        public async Task ObtenerArchivos() {
            
            var resp = await Global.ClientePrint.GetFiles();
            if (Global.ClientePrint.ResultRequest == EstadoRequest.Ok) {
                //Copia la lista de ficheros que se devuelve en la colección, para que por binding se muestre en el listview
                resp.ForEach(x => ficherosCollection.Add(x));
            }
            else {
                if (Global.ClientePrint.ResultRequest == EstadoRequest.SinConexion) {
                    await AvisoPerdidaConexion();
                }
            }
        }

        public async Task EnviarImprimir() {
            if (gcodeSeleccionado == null) {
                //Tratamiento, no se seleccionó ningún gcode para imprimir.
                await Application.Current.MainPage.DisplayAlert(AlertResources.PrintingHeaderError,
                            AlertResources.PrintingBodySelectOne,
                            AlertResources.PrintingReturn);
            }
            else {
                var estado = await Global.ClientePrint.PostImprimir(gcodeSeleccionado);
                if (estado == false) { //si no se pudo hacer, se comprueba por qué
                    if (Global.ClientePrint.ResultRequest == EstadoRequest.NoExiste) {
                        //Tratamiento de que no existe ese fichero
                    }
                    else if (Global.ClientePrint.ResultRequest == EstadoRequest.Ocupado) {
                        //Tratamiento de que la impresora se encuentra imprimiendo
                        await Application.Current.MainPage.DisplayAlert(AlertResources.PrintingHeaderError,
                            AlertResources.PrintingBodyProcessing,
                            AlertResources.PrintingReturn);
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                    else if (Global.ClientePrint.ResultRequest == EstadoRequest.SinConexPrinter) {
                        //No se pudo establecer conexión con la impresora
                    }
                }
                else { //todo va bien
                    //Retrocede a la página anterior, si se hizo de forma correcta.
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
            }
        }

        public async Task EliminarFichero() {
            if (gcodeSeleccionado == null) {
                //Tratamiento, no se seleccionó ningún gcode para eliminar
                await Application.Current.MainPage.DisplayAlert(AlertResources.DeletingHeader,
                            AlertResources.DeletingBodySelectOne,
                            AlertResources.PrintingReturn);
            }
            else {
                var estado = await Global.ClientePrint.DeleteFile(gcodeSeleccionado);
                ficherosCollection.Remove(gcodeSeleccionado);
                if (estado == false) {
                   //Cuando hubo error en la operación
                    if (Global.ClientePrint.ResultRequest == EstadoRequest.NoExiste) {
                        //Tratamiento de que no existe ese fichero
                    }
                    if (Global.ClientePrint.ResultRequest == EstadoRequest.Ocupado) {
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