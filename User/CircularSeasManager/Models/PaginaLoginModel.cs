using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using CircularSeasManager.Models;
using Xamarin.Essentials;
using System.Threading.Tasks;


namespace CircularSeasManager.Models{

    /*MODELO PAGINALOGIN: Se incorporan todos los datos relevantes e importantes en la página de inicio que sean
     * necesarios para trabajar*/
    public class PaginaLoginModel : BaseModel {


        //Propiedad para visualizar el resultado del inicio de sesión
        private string _mensajeinicio;
        public string MensajeInicio {
            get { return _mensajeinicio; }
            set {
                if (_mensajeinicio != value) {
                    _mensajeinicio = value;
                    OnPropertyChanged(); //Ahora si cambia la propiedad se notificará que se cambia la propiedad.
                }
            }
        }

        //Propiedad para el usuario que se introduce por teclado
        private string _usuario;
        public string Usuario {
            get { return _usuario; }
            set {
                _usuario = value;
                OnPropertyChanged();
            }
        }

        //Propiedad para la contraseña que se introduce
        private string _pass;
        public string Pass {
            get { return _pass; }
            set {
                _pass = value;
                OnPropertyChanged();
            }
        }

        public async Task GetSecureCredenciales() {
            Usuario = await SecureStorage.GetAsync("user");
            Pass = await SecureStorage.GetAsync("password");
            /*
            if (Preferences.ContainsKey("Recordarme")) {
                Recordarme = Preferences.Get("Recordarme", false);
            }*/
        }

        //Propiedad para indicar si quiero que se recuerde el usuario
        private bool _recordarme;
        public bool Recordarme {
            get { 
                if (Preferences.ContainsKey("Recordarme")) {
                    _recordarme = Preferences.Get("Recordarme", false);
                }
                return _recordarme; 
            }
            set {
                if (_recordarme != value) {
                    _recordarme = value;
                    Preferences.Set("Recordarme", _recordarme);
                }
                OnPropertyChanged();
            }
        }
    }
}
