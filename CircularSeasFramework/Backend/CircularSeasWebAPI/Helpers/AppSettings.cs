using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircularSeasWebAPI.Helpers
{
    public class AppSettings
    // Class that represent the appSettings.JSON file. It is contains the parameterizable paths of the application
    {
        public string stlFolderPath { get; set; }
        public string gCodeFolderPath { get; set; }
        public string prusaSlicerPath { get; set; }
        public string bundlesPath { get; set; }
        public string inisPath { get; set; }
        public string dataPath { get; set; }
        public string logPath { get; set; }
        public string DBConnectionString { get; set; }
    }
}
