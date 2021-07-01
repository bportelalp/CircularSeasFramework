using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircularSeasManager.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CircularSeasManager.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPrincipal : ContentPage {

        MenuPrincipalViewModel contexto = new MenuPrincipalViewModel();
        public MenuPrincipal() {
            InitializeComponent();
            //Enlaza el contexto de datos con el viewModel
            BindingContext = contexto;
        }


    }
}