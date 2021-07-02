using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models.DTO
{
    public class DataDTO
    {
        public Printer Printer { get; set; }
        public Filament[] Filaments { get; set; }
        public InfoTopsis InfoTopsis { get; set; }
    }
}
