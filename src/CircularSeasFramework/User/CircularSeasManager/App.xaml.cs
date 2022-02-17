using CircularSeasManager.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CircularSeasManager {
    public partial class App : Application {
        public App() {
            InitializeComponent();
            DependencyService.Register<Services.OctoClient>();
            DependencyService.Register<Services.SliceClient>();
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
