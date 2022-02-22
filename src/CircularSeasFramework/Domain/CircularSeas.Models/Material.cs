using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models
{
    public class Material
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double? BedTemperature { get; set; }
        public double? HotendTemperature { get; set; }

        //Data comes in array because they´re identified by position according to InfoTopsis data
        public int SpoolStock { get; set; }
        public bool Deprecated { get; set; }
        public List<Evaluation> Evaluations {get;set;}
    }
}
