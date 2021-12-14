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
    public class LoginPageModel : BaseModel {


        //Propiedad para visualizar el resultado del inicio de sesión
        private string _initMessage;
        public string InitMessage {
            get { return _initMessage; }
            set {
                if (_initMessage != value) {
                    _initMessage = value;
                    OnPropertyChanged(); //Ahora si cambia la propiedad se notificará que se cambia la propiedad.
                }
            }
        }

        //Propiedad para el usuario que se introduce por teclado
        private string _userInput;
        public string UserInput {
            get { return _userInput; }
            set {
                _userInput = value;
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
            UserInput = await SecureStorage.GetAsync("user");
            Pass = await SecureStorage.GetAsync("password");
            /*
            if (Preferences.ContainsKey("Recordarme")) {
                Recordarme = Preferences.Get("Recordarme", false);
            }*/
        }

        //Propiedad para indicar si quiero que se recuerde el usuario
        private bool _rememberme;
        public bool Rememberme {
            get { 
                if (Preferences.ContainsKey("Rememberme")) {
                    _rememberme = Preferences.Get("Rememberme", false);
                }
                return _rememberme; 
            }
            set {
                if (_rememberme != value) {
                    _rememberme = value;
                    Preferences.Set("Rememberme", _rememberme);
                }
                OnPropertyChanged();
            }
        }
    }
}
