using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircularSeasWebAPI.Classes
{
    public class Printer
    {
        public string Nombre { get; set; }
        public double Filament_diameter { get; set; }
        public string[] Profiles { get; set; }
    }
}
