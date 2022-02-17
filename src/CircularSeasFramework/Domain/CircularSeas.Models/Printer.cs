using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models
{
    public class Printer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double FilamentDiameter { get; set; }
        public List<string> Profiles { get; set; }
    }
}
