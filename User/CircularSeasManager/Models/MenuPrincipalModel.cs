using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using System.Runtime.CompilerServices;

namespace CircularSeasManager.Models {
    class MenuPrincipalModel : BaseModel {


        //Estado impresora, mostrado en pantalla
        private string _estadoImpresora;
        public string estadoImpresora {
            get { return _estadoImpresora; }
            set {
                if (_estadoImpresora != value) {
                    _estadoImpresora = value;
                    OnPropertyChanged();
                }
            }
        }

        //Nombre del fichero, mostrando en pantalla
        private string _nombreFichero;
        public string nombreFichero {
            get { return _nombreFichero; }
            set {
                if (_nombreFichero != value) {
                    _nombreFichero = value;
                    OnPropertyChanged();
                }
            }
        }

        //Porcentaje de trabajo realizadp
        private float _progreso;
        public float progreso {
            get { return _progreso; } //Lo devuelve en porcentaje
            set {
                if (_progreso != value) {
                    _progreso = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(progresoBar));
                }
            }
        }

        //Porcentaje en tanto por uno, para la toolbar (sólo admite get)
        public float progresoBar {
            get { return (_progreso / 100); } //Lo devuelve en porcentaje
        }

        //Temperatura del extrusor
        private float _HotendTemp;
        public float HotendTemp {
            get { return _HotendTemp; }
            set {
                if (_HotendTemp != value) {
                    _HotendTemp = value;
                    //OnPropertyChanged();
                    OnPropertyChanged(nameof(TempCadena)); //Notifica un cambio en la cadena
                }
            }
        }

        //Temperatura de la cama caliente
        private float _BedTemp;
        public float BedTemp {
            get { return _BedTemp; }
            set {
                if (_BedTemp != value) {
                    _BedTemp = value;
                    //OnPropertyChanged();
                    OnPropertyChanged(nameof(TempCadena)); //Notifica un cambio en la cadena
                }
            }
        }

        //Tiempo estimado de finalización
        private TimeSpan _printTimeLeft;
        public TimeSpan PrintTimeLeft {
            get { return _printTimeLeft; }
            set {
                if (_printTimeLeft != value) {
                    _printTimeLeft = value;
                    //OnPropertyChanged();
                    OnPropertyChanged(nameof(StringPrintTimeLeft));
                }
            }
        }

        //String de tempo restante estimado
        public string StringPrintTimeLeft {
            get { 
                return (PrintTimeLeft.Hours > 0? $"{PrintTimeLeft.Hours} hora"+ (PrintTimeLeft.Hours>1?"s":""):"") +
                            $" {PrintTimeLeft.Minutes} minuto" + (PrintTimeLeft.Minutes > 1 ? "s" : "");
            } 
        }
        
        public string TempCadena {
            get { return $"Extrusor: {_HotendTemp} ºC / Cama caliente: {_BedTemp} ºC"; }
        }

        private string _PausaResume = "Pausar";
        public string PausaResume {
            get { return _PausaResume; }
            set {
                if (_PausaResume != value) {
                    _PausaResume = value;
                    OnPropertyChanged();
                   
                }
            }
        }

        //Impresora conectada
        private bool _ImpresoraOffline;
        public bool ImpresoraOffline {
            get { return _ImpresoraOffline; }
            set {
                if (_ImpresoraOffline != value) {
                    _ImpresoraOffline = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
