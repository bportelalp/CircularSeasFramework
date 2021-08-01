using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Plugin.FilePicker.Abstractions;
using CircularSeasManager.Models;
using CircularSeasManager.Resources;

namespace CircularSeasManager.Models {
    public class SliceModel : BaseModel {

        //Colección de materiales
        public ObservableCollection<string> MaterialCollection { get; set; }

        //Colección de calidades
        public ObservableCollection<string> ProfileCollection { get; set; }

        //Almacén de todos los datos
        public CircularSeas.Models.DTO.DataDTO DataMaterial = new CircularSeas.Models.DTO.DataDTO();

        //Selección de Soporte
        private bool _useSupport;
        public bool UseSupport {
            get { return _useSupport; }
            set {
                if (_useSupport != value) {
                    _useSupport = value;
                    OnPropertyChanged();
                }
            }
        }

        //Material Seleccionado
        private string _materialSelected;
        public string MaterialSelected {
            get { return _materialSelected; }
            set {
                if (_materialSelected != value) {
                    _materialSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        //Calidad seleccionada
        private string _profileSelected;
        public string ProfileSelected {
            get { return _profileSelected; }
            set {
                if (_profileSelected != value) {
                    _profileSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        //Fichero STL seleccionado
        private FileData _STL;
        public FileData STL {
            get { return _STL; }
            set {
                if (_STL != value) {
                    _STL = value;
                    OnPropertyChanged(nameof(NameSTL));
                }
            }
        }

        //Ruta del STL
        public string NameSTL {
            get { return _STL == null ? StringResources.Empty : _STL.FileName; }
        }

        //Indica se están todolos items seleccionados
        public bool AllReady {
            get {
                if (string.IsNullOrEmpty(_profileSelected) || string.IsNullOrEmpty(_materialSelected) || (_STL == null)) {
                    return false;
                }
                else { return true; }
            }
        }

        //Mensaje de pantalla
        private string _statusMessage;
        public string StatusMessage {
            get { return _statusMessage; }
            set {
                if (_statusMessage != value) {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

    }
}
