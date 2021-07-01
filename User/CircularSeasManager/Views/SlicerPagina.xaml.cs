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
    public partial class SlicerPagina : ContentPage {

        public SliceViewModel contexto = new SliceViewModel();
        public SlicerPagina() {
            InitializeComponent();
            BindingContext = contexto;
        }

        protected override void OnAppearing() {
            contexto.materialSelected = Global.MaterialRecomendado;
            base.OnAppearing();
        }
    }
}