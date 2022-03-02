using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CircularSeasManager.Models;
using CircularSeasManager.ViewModels;

namespace CircularSeasManager.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage {

        ConfigViewModel context = new ConfigViewModel();
        public SettingsPage() {
            InitializeComponent();
            BindingContext = context;
        }
    }
}