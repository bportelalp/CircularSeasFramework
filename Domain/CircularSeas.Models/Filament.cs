using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models
{
    public class Filament
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool[] FeaturesValues { get; set; }
        public double[] PropertiesValues { get; set; }
        public int SpoolStock { get; set; }
    }
}
