using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using CircularSeasManager.Resources;

namespace CircularSeasManager.Models {
    class MainPanelModel : BaseModel {


        //Estado impresora, mostrado en pantalla
        private string _printerState;
        public string PrinterState {
            get { return _printerState; }
            set {
                if (_printerState != value) {
                    _printerState = value;
                    OnPropertyChanged();
                }
            }
        }

        //Nombre del fichero, mostrando en pantalla
        private string _fileName;
        public string FileName {
            get { return _fileName; }
            set {
                if (_fileName != value) {
                    _fileName = value;
                    OnPropertyChanged();
                }
            }
        }

        //Porcentaje de trabajo realizadp
        private float _progress;
        public float Progress {
            get { return _progress; } //Lo devuelve en porcentaje
            set {
                if (_progress != value) {
                    _progress = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ProgressBar));
                }
            }
        }

        //Porcentaje en tanto por uno, para la toolbar (sólo admite get)
        public float ProgressBar {
            get { return _progress / 100; } //Lo devuelve en porcentaje
        }

        //Temperatura del extrusor
        private float _HotendTemp;
        public float HotendTemp {
            get { return _HotendTemp; }
            set {
                if (_HotendTemp != value) {
                    _HotendTemp = value;
                    //OnPropertyChanged();
                    OnPropertyChanged(nameof(StringTemperature)); //Notifica un cambio en la cadena
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
                    OnPropertyChanged(nameof(StringTemperature)); //Notifica un cambio en la cadena
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
                return (PrintTimeLeft.Hours > 0 ? $"{PrintTimeLeft.Hours}" + (PrintTimeLeft.Hours > 1 ? $" {StringResources.hours}" : $" {StringResources.hour}") : "") +
                            $" {PrintTimeLeft.Minutes}" + (PrintTimeLeft.Minutes > 1 ? $" {StringResources.minutes}" : $" {StringResources.minute}");
            } 
        }
        
        public string StringTemperature {
            get { return $"{StringResources.Extruder}: {_HotendTemp} ºC / {StringResources.Hotbed}: {_BedTemp} ºC"; }
        }

        private string _PauseOrResume = StringResources.Pause;
        public string PauseOrResume {
            get { return _PauseOrResume; }
            set {
                if (_PauseOrResume != value) {
                    _PauseOrResume = value;
                    OnPropertyChanged();
                   
                }
            }
        }

        //Impresora conectada
        private bool _printerOffline;
        public bool PrinterOffline {
            get { return _printerOffline; }
            set {
                if (_printerOffline != value) {
                    _printerOffline = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
