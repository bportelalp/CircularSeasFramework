using System;
using System.Collections.Generic;
using System.Text;
using CircularSeasManager.Services;

namespace CircularSeasManager.Models {
    public static class Global {

        //Instancia estática del cliente rest que se usará en todo el proyecto.
        public static Services.OctoCliente ClientePrint;
        public static Services.SliceCliente ClienteSlice;
        public static string MaterialRecomendado;
    }
}
