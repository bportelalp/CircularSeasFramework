using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CircularSeasManager.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Threading;
using CircularSeasManager.Resources;

namespace CircularSeasManager.ViewModels {
    public class MaterialAssistantViewModel : MaterialAssistantModel {
        
        //Comando para calcular.
        public Command CmdSugerir { get; set; }
        //Comando para aceptar material
        public Command CmdAceptarMaterial { get; set; }
        //Comando para llamar por teléfono
        public Command CmdAyuda { get; set; }

        public MaterialAssistantViewModel(CircularSeas.Models.DTO.DataDTO _material) {

            DataMaterial = _material;
            
            //Instanciar colección con criterios para visualizar en View.
            ValueUserCollection = new ObservableCollection<ValueUser>();
            foreach (string item in DataMaterial.InfoTopsis.PropertiesNames) {
                ValueUserCollection.Add(new ValueUser { Propiedade = item, Valoracion = 0.1 });
            }

            //Instanciar features
            FeaturesUserCollection = new ObservableCollection<FeaturesUser>();
            List<string> opciones = new List<string>();
            opciones.Add(StringResources.Yes); opciones.Add(StringResources.No); opciones.Add(StringResources.NotApplicable);
            foreach (string item in DataMaterial.InfoTopsis.FeaturesNames) {
                FeaturesUserCollection.Add(new FeaturesUser {
                    feature = item,
                    optionsFeature = new ObservableCollection<string>(opciones),
                    featureValueSelected = StringResources.NotApplicable
                });
            }
            //Instanciar colección de resultados
            ResultadoTOPSISCollection = new ObservableCollection<ResultadoTOPSIS>();

            //Comando para calcular sugerencia de material
            CmdSugerir = new Command(() => Sugerir(), () => !Ocupado);
            CmdInfo = new Command(() => InformarMaterial(), () => !Ocupado);
            CmdAceptarMaterial = new Command(async () => await AceptarMaterial(), () => !Ocupado);
            CmdAyuda = new Command(() => Ayuda());
        }

        public void Sugerir() {
            List<int> cribado = new List<int>();
            foreach (var item in FeaturesUserCollection) {
                if (item.featureValueSelected == StringResources.Yes) {
                    cribado.Add(1);
                }
                else if (item.featureValueSelected == StringResources.No) {
                    cribado.Add(0);
                }
                else {
                    cribado.Add(2);
                }
            }

            //Cargar matriz de datos para algoritmo TOPSIS
            double[,] datos = new double[DataMaterial.Filaments.Length, DataMaterial.InfoTopsis.PropertiesNames.Length];
            for (int i = 0; i < DataMaterial.Filaments.Length; i++) {
                for (int j = 0; j < DataMaterial.InfoTopsis.PropertiesNames.Length; j++) {
                    datos[i, j] = DataMaterial.Filaments[i].PropertiesValues[j];
                }
            }
            //Cargar resultado decisión de los sliders
            double[] decision = new double[DataMaterial.InfoTopsis.PropertiesNames.Length];
            int k = 0;
            foreach (ValueUser item in ValueUserCollection) {
                decision[k] = item.Valoracion;
                k++;
            }
            //Instanciar array de resultado
            double[] recomendacion = new double[DataMaterial.InfoTopsis.PropertiesNames.Length];
            //Calcular y cargar en colección
            recomendacion = TOPSIS(datos, decision, new bool[4] { true, true, true, true });
            ResultadoTOPSISCollection.Clear();
            for (int i = 0; i < recomendacion.Length; i++) {
                bool pasacribado = true;
                for (int j = 0; j < DataMaterial.Filaments[i].FeaturesValues.Length; j++) {
                    if (cribado[j] == 2) {
                        //Si es "no importa, todo ok
                    }
                    else if (cribado[j] == 1 & DataMaterial.Filaments[i].FeaturesValues[j] == true) {
                        //Si son las dos afirmativas, pasa
                    }
                    else if (cribado[j] == 0 & DataMaterial.Filaments[i].FeaturesValues[j] == false) {
                        //Si son las dos negativas, pasa
                    }
                    else {
                        //Si no coinciden no pasa
                        pasacribado = false;
                        break;
                    }
                }
                if (pasacribado) {
                    ResultadoTOPSISCollection.Add(new ResultadoTOPSIS {
                        NomeMaterial = DataMaterial.Filaments[i].Name,
                        Afinidade = recomendacion[i],
                        Afinidade100 = recomendacion[i] * 100.0,
                        Stock = (DataMaterial.Filaments[i].SpoolStock > 0) ? (DataMaterial.Filaments[i].SpoolStock + " in stock") : "Out of stock"
                    });
                }
            }

            Ocupado = false;
            HayResultado = true;


        }

        public void InformarMaterial() {
            //Buscar la descripción para el material seleccionado
            for (int i = 0; i < DataMaterial.Filaments.Length; i++) {
                if (DataMaterial.Filaments[i].Name == MaterialSeleccionado.NomeMaterial) {
                    InfoMaterial = DataMaterial.Filaments[i].Description;
                    break;
                }
            }
        }

        public async Task AceptarMaterial() {
            if (MaterialSeleccionado != null) {
                foreach (var item in DataMaterial.Filaments) {
                    if (item.Name == MaterialSeleccionado.NomeMaterial) {
                        if (item.SpoolStock == 0) {
                            var decision = await Application.Current.MainPage.DisplayAlert("Sin stock", "No hay stock actual para este material", "Pedir", "Abortar");
                            if (decision) {
                                PhoneDialer.Open("689356647");
                            }
                            else {
                                break;
                            }
                        }
                        else {
                            Global.MaterialRecomendado = MaterialSeleccionado.NomeMaterial;
                            await Application.Current.MainPage.Navigation.PopAsync();
                            break;
                        }
                    }
                }

            }
            else {
                await Application.Current.MainPage.DisplayAlert("Error", "Debes seleccionar un material", "Entendido");
            }
        }

        public void Ayuda() {
            PhoneDialer.Open("689356647");
        }

    }
}
