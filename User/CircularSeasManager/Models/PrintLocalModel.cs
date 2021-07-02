using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace CircularSeasManager.Models {
    class PrintLocalModel : BaseModel {

        //Colección para mostrar en el listview, que se listan todos los archivos gcode del documento.
        public ObservableCollection<string> ficherosCollection { get; set; } //Colección para listar los ficheros.

        //Enlace de datos con el item seleccionado.
        private string _gcodeSeleccionado;
        public string gcodeSeleccionado {
            get { return _gcodeSeleccionado; }
            set {
                if (_gcodeSeleccionado != value) {
                    _gcodeSeleccionado = value;
                    if (_gcodeSeleccionado == "") {
                        IsSelectedGCODE = false;
                    }
                    else { IsSelectedGCODE = true;  }
                    OnPropertyChanged();
                }
            }
        }

        //Boleano que indica si hay un elemento del listview seleccionado, útil para el viewmodel.
        private bool _IsSelectedGCODE;
        public bool IsSelectedGCODE {
            get { return _IsSelectedGCODE; }
            set {
                if ( _IsSelectedGCODE != value) {
                    _IsSelectedGCODE = value;
                }
                OnPropertyChanged();
            }
        }





    }
}
