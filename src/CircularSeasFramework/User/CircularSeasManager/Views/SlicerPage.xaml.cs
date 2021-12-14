using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircularSeasManager.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CircularSeasManager.Models;

namespace CircularSeasManager.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlicerPage : ContentPage {

        public SliceViewModel context = new SliceViewModel();
        public SlicerPage() {
            InitializeComponent();
            BindingContext = context;
        }

        protected override void OnAppearing() {
            context.MaterialSelected = Global.RecommendedMaterial;
            base.OnAppearing();
        }
    }
}