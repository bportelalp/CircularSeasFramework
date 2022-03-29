using CircularSeasManager.Views;
using System;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CircularSeasManager {
    public partial class App : Application {
        public App() {
            InitializeComponent();

            // Inyección de los servicios necesarios en las vista-modelo
            DependencyService.Register<Services.OctoClient>();
            DependencyService.Register<Services.SliceClient>();
            DependencyService.Register<System.Net.Http.HttpClient>();
            DependencyService.Register<Services.IQrService>();

            //Punto de inicio
            MainPage = new NavigationPage(new LoginPage());
        }

        protected override void OnStart() {
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}
