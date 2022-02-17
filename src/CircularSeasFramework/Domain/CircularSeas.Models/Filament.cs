using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models
{
    public class Filament
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double? BedTemperature { get; set; }
        public double? HotendTemperature { get; set; }
        public double? IdealTempExtr { get; set; }
        public double? MinTempExtr { get; set; }
        public double? MaxTempExtr { get; set; }

        //Data comes in array because they´re identified by position according to InfoTopsis data
        public bool[] FeaturesValues { get; set; }
        public double[] PropertiesValues { get; set; }
        public int SpoolStock { get; set; }
    }
}
