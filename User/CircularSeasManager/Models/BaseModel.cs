using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace CircularSeasManager.Models {

    /// <summary>
    /// Clase con elementos genéricos y que implementa la interfaz INotifyPropertyChanged para notificar eventos
    /// </summary>
    public class BaseModel : INotifyPropertyChanged {

        //Evento que se produce cuando cambia propiedad
        public event PropertyChangedEventHandler PropertyChanged;

        //Comando común para todas las páginas
        public Command CmdConfig { get; set; }

        public void OnPropertyChanged([CallerMemberName] string nombre = "") {
            /*A este método se le manda el nombre de la propiedad, con callerMemberName el sistema obtiene la propiedad
            (sobre la cual dentro de, se llamó a esto)*/
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
            /*Creo un método que cuando cambia una propiedad se invoca. Los parámetros:
            - this, hace referencia a que uso este modelo (esta clase)
            new PropertyChangedEventArgs, se indica el nombre de la propiedad que cambia, pasada por parámetro*/
        }


        //Propiedad para mostrar estado de solicitud, normalmente gestionando un ActivityIndicator
        private bool _ocupado = false;
        public bool Ocupado {
            get { return _ocupado; }
            set {
                _ocupado = value;
                OnPropertyChanged();
            }
        }

        //Propiedad que notifica que se mantiene en la página en curso
        public bool InPage { get; set; }

        public async Task AvisoPerdidaConexion() {
            var opcion = await Application.Current.MainPage.DisplayAlert("Sin conexión", "Se ha perdido la conexión con Octoprint", "Volver a inicio", "esperar");
            if (opcion) {
                Application.Current.MainPage = new NavigationPage(new Views.LoginPage());
            }
            else {
                //Nada
            }
        }

        /// <summary>
        /// Abre la página de configuración
        /// </summary>
        public async Task IrAConfig() {
            //Abrir nueva página de configuración
            await Application.Current.MainPage.Navigation.PushAsync(new Views.SettingsPage());
        }

        //Preferencia común IP, utiliza el contenedor interno con clave IP para almacenarlo.
        private string _IPOctoprint = "http://circularseasprinter.local";
        public string IPOctoprint {
            get {
                if (Preferences.ContainsKey("IPOctoprint")) {
                    _IPOctoprint = Preferences.Get("IPOctoprint","http://circularseasprinter.local");
                }
                return _IPOctoprint;
            }
            set {
                if (_IPOctoprint != value) {
                    _IPOctoprint =  value;
                    Preferences.Set("IPOctoprint", _IPOctoprint);
                    OnPropertyChanged();
                }
            }
        }

        //Preferencia común IP, utiliza el contenedor interno con clave IP para almacenarlo.
        private string _IPSlicer = "http://192.168.0.10";
        public string IPSlicer {
            get {
                if (Preferences.ContainsKey("IPSlicer")) {
                    _IPSlicer = Preferences.Get("IPSlicer", "http://192.168.0.10");
                }
                return _IPSlicer;
            }
            set {
                if (_IPSlicer != value) {
                    _IPSlicer = value;
                    Preferences.Set("IPSlicer", _IPSlicer);
                    OnPropertyChanged();
                }
            }
        }

        //Almacena el nombre de la impresora, en las preferencias
        private string _printer;
        public string printer {
            get {
                if (Preferences.ContainsKey("Printer")) {
                    _printer = Preferences.Get("Printer", "");
                }
                return _printer;
            }
            set {
                if (_printer != value) {
                    _printer = value;
                    Preferences.Set("Printer", _printer);
                    OnPropertyChanged();
                }
            }
        }
    }
}
