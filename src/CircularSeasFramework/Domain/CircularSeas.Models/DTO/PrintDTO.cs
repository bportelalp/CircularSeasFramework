using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models.DTO
{
    public class PrintDTO
    {
        public List<Models.Material> Materials { get; set; }
        public List<Models.Slicer.Filament> Filaments { get; set; }
    }
}
