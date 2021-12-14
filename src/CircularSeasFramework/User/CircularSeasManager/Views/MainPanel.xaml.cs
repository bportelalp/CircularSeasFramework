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
    public partial class MainPanel : ContentPage {

        MainPanelViewModel context = new MainPanelViewModel();
        public MainPanel() {
            InitializeComponent();
            //Enlaza el context de datos con el viewModel
            BindingContext = context;
        }


    }
}