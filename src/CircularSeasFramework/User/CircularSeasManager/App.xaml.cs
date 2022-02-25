using CircularSeasManager.Views;
using System;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CircularSeasManager {
    public partial class App : Application {
        public App() {
            InitializeComponent();
            DependencyService.Register<Services.OctoClient>();
            DependencyService.Register<Services.SliceClient>();
            DependencyService.Register<System.Net.Http.HttpClient>();
            DependencyService.Register<Services.IQrService>();
            MainPage = new NavigationPage(new LoginPage());
            //MainPage = new NavigationPage(new OrderPage());
        }

        protected override void OnStart() {
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}
