using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircularSeasManager.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CircularSeasManager.Models;

namespace CircularSeasManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlicerPage : ContentPage
    {

        public SliceViewModel context = new SliceViewModel();
        public SlicerPage()
        {
            InitializeComponent();
            BindingContext = context;
        }

        protected override void OnAppearing()
        {
            if (Global.RecommendedMaterialId != default(Guid) && context.MaterialCollection.Any())
            {
                if (context.MaterialCollection.Select(mc => mc.Id).Contains(Global.RecommendedMaterialId))
                {
                    context.MaterialSelected = context.MaterialCollection.Where(s => s.Id == Global.RecommendedMaterialId).FirstOrDefault();
                }
            }
            base.OnAppearing();
        }
    }
}