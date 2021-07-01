using CircularSeasManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace CircularSeasManager.Views {

    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class PaginaLogin : ContentPage {

        PaginaLoginViewModel contexto = new PaginaLoginViewModel();
        public PaginaLogin() {
            InitializeComponent();
            //Enlaza el contexto de datos con el viewModel
            BindingContext = contexto;
        }

        protected override async void OnAppearing() {
            //Restaura al aparecer los parámetros almacenados en el secure storage, asociándolos con su contexto.
            /*contexto.Usuario = await SecureStorage.GetAsync("user");
            contexto.Pass = await SecureStorage.GetAsync("password");
            if (Preferences.ContainsKey("Recordarme")) {
                contexto.Recordarme = Preferences.Get("Recordarme", false);
            }*/
            base.OnAppearing();
        }
    }
}