using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Plugin.FilePicker.Abstractions;
using CircularSeasManager.Models;
using CircularSeasManager.Resources;
using System.Linq;

namespace CircularSeasManager.Models {
    public class SliceModel : BaseModel {

        //Colección de materiales
        public ObservableCollection<CircularSeas.Models.Material> MaterialCollection { get; set; }

        //Colección de calidades
        public ObservableCollection<CircularSeas.Models.Slicer.Print> ProfileCollection { get; set; }

        //Almacén de todos los datos
        public CircularSeas.Models.DTO.PrintDTO DataMaterial = new CircularSeas.Models.DTO.PrintDTO();

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
        private CircularSeas.Models.Material _materialSelected;
        public CircularSeas.Models.Material MaterialSelected {
            get { return _materialSelected; }
            set {
                if (_materialSelected != value) {
                    _materialSelected = value;
                    ProfileCollection.Clear();
                    foreach(var print in DataMaterial.Filaments.Find(f => f.MaterialFK == _materialSelected.Id).CompatiblePrints)
                    {
                        ProfileCollection.Add(print);
                    }
                    OnPropertyChanged();
                }
            }
        }

        //Calidad seleccionada
        private CircularSeas.Models.Slicer.Print _profileSelected;
        public CircularSeas.Models.Slicer.Print ProfileSelected {
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
                if (_profileSelected == null || _materialSelected == null || (_STL == null)) {
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
