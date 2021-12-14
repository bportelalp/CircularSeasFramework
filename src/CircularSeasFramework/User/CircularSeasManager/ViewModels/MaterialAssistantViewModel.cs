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
        public Command CmdAcceptMaterial { get; set; }
        //Comando para llamar por teléfono
        public Command CmdHelp { get; set; }

        public MaterialAssistantViewModel(CircularSeas.Models.DTO.DataDTO _material) {

            DataMaterial = _material;
            
            //Instanciar colección con criterios para visualizar en View.
            ValueUserCollection = new ObservableCollection<ValueUser>();
            foreach (string item in DataMaterial.InfoTopsis.PropertiesNames) {
                ValueUserCollection.Add(new ValueUser { Property = item, Valoration = 0.1 });
            }

            //Instanciar features
            FeaturesUserCollection = new ObservableCollection<FeaturesUser>();
            List<string> opciones = new List<string>();
            opciones.Add(StringResources.Yes); opciones.Add(StringResources.No); opciones.Add(StringResources.NotApplicable);
            foreach (string item in DataMaterial.InfoTopsis.FeaturesNames) {
                FeaturesUserCollection.Add(new FeaturesUser {
                    Feature = item,
                    OptionsFeature = new ObservableCollection<string>(opciones),
                    FeatureValueSelected = StringResources.NotApplicable
                });
            }
            //Instanciar colección de resultados
            TOPSISResultCollection = new ObservableCollection<TOPSISResult>();

            //Comando para calcular sugerencia de material
            CmdSugerir = new Command(() => Suggest(), () => !Busy);
            CmdInfo = new Command(() => LocateInfoMaterialSelected(), () => !Busy);
            CmdAcceptMaterial = new Command(async () => await AcceptMaterial(), () => !Busy);
            CmdHelp = new Command(() => Help());
        }

        public void Suggest() {
            //ETAPA 1: Recoge los datos del filtro inicial en una lista
            List<int> startingFilter = new List<int>();
            foreach (var item in FeaturesUserCollection) {
                if (item.FeatureValueSelected == StringResources.Yes) {
                    startingFilter.Add(1);
                }
                else if (item.FeatureValueSelected == StringResources.No) {
                    startingFilter.Add(0);
                }
                else {
                    startingFilter.Add(2);
                }
            }

            //ETAPA 2: Prepara ejecución de TOPSIS
            //Cargar matriz de decisión.
            double[,] decisionMatrix = new double[DataMaterial.Filaments.Length, DataMaterial.InfoTopsis.PropertiesNames.Length];
            for (int i = 0; i < DataMaterial.Filaments.Length; i++) {
                for (int j = 0; j < DataMaterial.InfoTopsis.PropertiesNames.Length; j++) {
                    decisionMatrix[i, j] = DataMaterial.Filaments[i].PropertiesValues[j];
                }
            }
            //Cargar resultado decisión de los sliders
            double[] performanceUser = new double[DataMaterial.InfoTopsis.PropertiesNames.Length];
            int k = 0;
            foreach (ValueUser item in ValueUserCollection) {
                performanceUser[k] = item.Valoration;
                k++;
            }
            //Cargar valoracion de impacto de materiales
            bool[] impactPositive = DataMaterial.InfoTopsis.ImpactPositive;
            //Instanciar array de resultado
            double[] result = new double[DataMaterial.InfoTopsis.PropertiesNames.Length];
            //Calcular y cargar en colección
            result = TOPSIS(decisionMatrix, performanceUser, impactPositive);
            TOPSISResultCollection.Clear();

            //ETAPA3: Acepta solo los resultados que coincidan con el cribado inicial
            for (int i = 0; i < result.Length; i++) {
                bool startingFilterPassed = true;
                for (int j = 0; j < DataMaterial.Filaments[i].FeaturesValues.Length; j++) {
                    if (startingFilter[j] == 2) {
                        //Si es "no importa, todo ok
                    }
                    else if (startingFilter[j] == 1 & DataMaterial.Filaments[i].FeaturesValues[j] == true) {
                        //Si son las dos afirmativas, pasa
                    }
                    else if (startingFilter[j] == 0 & DataMaterial.Filaments[i].FeaturesValues[j] == false) {
                        //Si son las dos negativas, pasa
                    }
                    else {
                        //Si no coinciden no pasa
                        startingFilterPassed = false;
                        break;
                    }
                }
                //Si cumple, se carga en colección de resultado
                if (startingFilterPassed) {
                    TOPSISResultCollection.Add(new TOPSISResult {
                        MaterialName = DataMaterial.Filaments[i].Name,
                        Affinity = result[i],
                        Affinity100 = result[i] * 100.0,
                        Stock = (DataMaterial.Filaments[i].SpoolStock > 0) ? (DataMaterial.Filaments[i].SpoolStock + $" {StringResources.InStock}") : StringResources.OutStock
                    });
                }
            }
            //Ordenes para mostrar en pantalla
            Busy = false;
            HaveResult = true;
        }

        public void LocateInfoMaterialSelected() {
            //Buscar la descripción para el material seleccionado
            for (int i = 0; i < DataMaterial.Filaments.Length; i++) {
                if (DataMaterial.Filaments[i].Name == SelectedMaterial.MaterialName) {
                    InfoMaterial = DataMaterial.Filaments[i].Description;
                    break;
                }
            }
        }

        public async Task AcceptMaterial() {
            if (SelectedMaterial != null) {
                foreach (var item in DataMaterial.Filaments) {
                    if (item.Name == SelectedMaterial.MaterialName) {
                        if (item.SpoolStock == 0) {
                            var decision = await Application.Current.MainPage.DisplayAlert(AlertResources.OutStock,
                                AlertResources.OutStockMessage,
                                AlertResources.OrderStock,
                                AlertResources.PrintingReturn);
                            if (decision) {
                                await Browser.OpenAsync("https://circularseas.com/es/inicio-2/");
                            }
                            else {
                                break;
                            }
                        }
                        else {
                            Global.RecommendedMaterial = SelectedMaterial.MaterialName;
                            await Application.Current.MainPage.Navigation.PopAsync();
                            break;
                        }
                    }
                }

            }
            else {
                await Application.Current.MainPage.DisplayAlert(AlertResources.Error,
                    AlertResources.SelectMaterial,
                    AlertResources.Accept);
            }
        }

        public void Help() {
            PhoneDialer.Open("986812000");
        }

    }
}
