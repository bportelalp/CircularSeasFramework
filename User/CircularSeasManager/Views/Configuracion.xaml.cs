using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CircularSeasManager.Models;

namespace CircularSeasManager.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Configuracion : ContentPage {

        BaseModel contexto = new BaseModel();
        public Configuracion() {
            InitializeComponent();
            BindingContext = contexto;
        }
    }
}