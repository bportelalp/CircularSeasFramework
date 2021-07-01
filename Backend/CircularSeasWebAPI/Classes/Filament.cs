using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircularSeasWebAPI.Classes
{
    public class Filament
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool[] Features { get; set; }
        public double[] Properties { get; set; }
        public int Stock { get; set; }
    }
}
