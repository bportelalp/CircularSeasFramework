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

    public partial class LoginPage : ContentPage {

        LoginPageViewModel context = new LoginPageViewModel();
        public LoginPage() {
            InitializeComponent();
            //Enlaza el context de datos con el viewModel
            BindingContext = context;
        }

        protected override async void OnAppearing() {
            //Restaura al aparecer los parámetros almacenados en el secure storage, asociándolos con su context.
            /*context.Usuario = await SecureStorage.GetAsync("user");
            context.Pass = await SecureStorage.GetAsync("password");
            if (Preferences.ContainsKey("Recordarme")) {
                context.Recordarme = Preferences.Get("Recordarme", false);
            }*/
            base.OnAppearing();
        }
    }
}