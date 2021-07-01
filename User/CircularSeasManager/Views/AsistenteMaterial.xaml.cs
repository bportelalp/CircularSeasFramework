using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CircularSeasManager.ViewModels;
using CircularSeasManager.Models;

namespace CircularSeasManager.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AsistenteMaterial : ContentPage {

        AsistenteMaterialviewModel contexto;
        public AsistenteMaterial(InfoTopsis _material) {
            InitializeComponent();
            contexto = new AsistenteMaterialviewModel(_material);
            BindingContext = contexto;

        }

    }
}