using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models.Slicer
{
    public class Print
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string iniKeyword { get; set; }
    }

    public class Filament
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string iniKeyword { get; set; }
        public Guid MaterialFK { get; set; }
        public Models.Material Material { get; set; }
        public List<Print> CompatiblePrints { get; set; }
    }

}
