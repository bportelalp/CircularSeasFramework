using System;
using System.Collections.Generic;
using System.Text;
using CircularSeasManager.Services;

namespace CircularSeasManager.Models {
    public static class Global {

        //Instancia estática del cliente rest que se usará en todo el proyecto.
        public static Services.OctoClient PrinterClient;
        public static Services.SliceClient ClienteSlice;
        public static string RecommendedMaterial;
    }
}
