using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using CircularSeasManager.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Threading;
using CircularSeasManager.Resources;

namespace CircularSeasManager.ViewModels
{
    public class MaterialAssistantViewModel : MaterialAssistantModel
    {

        //Comando para calcular.
        public Command CmdSugerir { get; set; }
        //Comando para aceptar material
        public Command CmdAcceptMaterial { get; set; }
        //Comando para llamar por teléfono
        public Command CmdHelp { get; set; }

        public MaterialAssistantViewModel(CircularSeas.Models.DTO.PrintDTO printInfo)
        {

            DataMaterial = printInfo;

            var properties = DataMaterial.Materials.FirstOrDefault().Evaluations.Select(m => m.Property).ToList();
            //Instanciar colección con criterios para visualizar en View.
            ValueUserCollection = new ObservableCollection<ValueUser>();
            foreach (CircularSeas.Models.Property item in properties.Where(p => !p.IsDichotomous))
            {
                ValueUserCollection.Add(new ValueUser { Property = item, Valoration = 0.1 });
            }

            //Instanciar features
            FeaturesUserCollection = new ObservableCollection<FeaturesUser>();
            List<string> opciones = new List<string>();
            opciones.Add(StringResources.Yes); opciones.Add(StringResources.No); opciones.Add(StringResources.NotApplicable);
            foreach (CircularSeas.Models.Property item in properties.Where(p => p.IsDichotomous))
            {
                FeaturesUserCollection.Add(new FeaturesUser
                {
                    PropertyDisc = item,
                    OptionsFeature = new ObservableCollection<string>(opciones),
                    OptionSelected = StringResources.NotApplicable
                });
            }
            //Instanciar colección de resultados
            TOPSISResultCollection = new ObservableCollection<TOPSISResult>();

            //Comando para calcular sugerencia de material
            CmdSugerir = new Command(() => Suggest(), () => !Busy);
            CmdAcceptMaterial = new Command(async () => await AcceptMaterial(), () => !Busy);
            CmdHelp = new Command(() => Help());
        }

        public void Suggest()
        {
            //ETAPA 2: Prepara ejecución de TOPSIS
            //Cargar matriz de decisión.
            double[,] decisionMatrix = new double[DataMaterial.Materials.Count, ValueUserCollection.Count];
            for (int i = 0; i < DataMaterial.Materials.Count; i++)
            {
                for (int j = 0; j < ValueUserCollection.Count; j++)
                {
                    decisionMatrix[i, j] = (double)DataMaterial.Materials[i].Evaluations.Where(e => e.Property.Id == ValueUserCollection[j].Property.Id).FirstOrDefault().ValueDec;
                }
            }
            //Cargar resultado decisión de los sliders
            double[] performanceUser = new double[ValueUserCollection.Count];
            int k = 0;
            foreach (ValueUser item in ValueUserCollection)
            {
                performanceUser[k] = item.Valoration;
                k++;
            }
            //Cargar valoracion de impacto de materiales
            bool[] impactPositive = ValueUserCollection.Select(e => (bool)e.Property.MoreIsBetter).ToArray();
            //Instanciar array de resultado
            double[] result = new double[ValueUserCollection.Count];
            //Calcular y cargar en colección
            result = TOPSIS(decisionMatrix, performanceUser, impactPositive);
            TOPSISResultCollection.Clear();

            //ETAPA3: Acepta solo los resultados que coincidan con el cribado inicial
            for (int i = 0; i < result.Length; i++)
            {
                TOPSISResultCollection.Add(new TOPSISResult
                {
                    Material = DataMaterial.Materials[i],
                    Affinity = result[i],
                    Stock = (DataMaterial.Materials[i].SpoolStock > 0) ? (DataMaterial.Materials[i].SpoolStock + $" {StringResources.InStock}") : StringResources.OutStock
                });
            }

            var mat2delete = new List<Guid>();
            foreach (var feature in FeaturesUserCollection)
            {
                foreach(var item in TOPSISResultCollection)
                {
                    if (feature.OptionSelected == StringResources.Yes && !(bool)item.Material.Evaluations.Find(e => e.Property.Id == feature.PropertyDisc.Id).ValueBin)
                    {
                        mat2delete.Add(item.Material.Id);
                    }
                    else if (feature.OptionSelected == StringResources.No && (bool)item.Material.Evaluations.Find(e => e.Property.Id == feature.PropertyDisc.Id).ValueBin)
                    {
                        mat2delete.Add(item.Material.Id);
                    }
                }
            }
            foreach (var item in mat2delete)
            {
                TOPSISResultCollection.Remove(TOPSISResultCollection.Where(r => r.Material.Id == item).FirstOrDefault());
            }
            //Ordenes para mostrar en pantalla
            Busy = false;
            HaveResult = true;
        }

        public async Task AcceptMaterial()
        {
            if (SelectedMaterial != null)
            {
                if (SelectedMaterial.Material.Name == SelectedMaterial.MaterialName)
                {
                    if (SelectedMaterial.Material.SpoolStock == 0)
                    {
                        var decision = await Application.Current.MainPage.DisplayAlert(AlertResources.OutStock,
                            AlertResources.OutStockMessage,
                            AlertResources.OrderStock,
                            AlertResources.PrintingReturn);
                        if (decision)
                        {
                            await Browser.OpenAsync("https://circularseas.com/es/inicio-2/");
                        };
                    }
                    else
                    {
                        Global.RecommendedMaterial = SelectedMaterial.MaterialName;
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(AlertResources.Error,
                    AlertResources.SelectMaterial,
                    AlertResources.Accept);
            }
        }

        public void Help()
        {
            PhoneDialer.Open("986812000");
        }
    }
}
