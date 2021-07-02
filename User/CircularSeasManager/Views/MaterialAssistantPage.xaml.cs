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
    public partial class MaterialAssistantPage : ContentPage {

        MaterialAssistantViewModel context;
        public MaterialAssistantPage(CircularSeas.Models.DTO.DataDTO _material) {
            InitializeComponent();
            context = new MaterialAssistantViewModel(_material);
            BindingContext = context;

        }

    }
}