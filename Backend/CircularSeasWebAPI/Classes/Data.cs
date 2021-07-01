using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircularSeasWebAPI.Classes
{
    public class Data
    {
        public Printer Printer { get; set; }
        public Filament[] Filaments { get; set; }
        public InfoTopsis InfoTopsis { get; set; }
    }
}
