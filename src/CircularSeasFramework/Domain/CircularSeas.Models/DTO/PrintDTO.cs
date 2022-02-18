using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models.DTO
{
    public class PrintDTO
    {
        public Models.Printer Printer { get; set; }
        public List<Models.Material> Materials { get; set; }
    }
}
