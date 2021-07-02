using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models
{
    public class InfoTopsis
    {
        public Printer printer { get; set; }
        public string[] FeaturesNames { get; set; }
        public string[] PropertiesNames { get; set; }
        public bool[] ImpactPositive { get; set; }
        public Filament[] Filaments { get; set; }
    }
}
