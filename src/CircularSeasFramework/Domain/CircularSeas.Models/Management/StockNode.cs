using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models.Management
{
    public class StockNode
    {
        public string NodeName { get; set; }
        public string IsProvider { get; set; }
        public List<Models.Material> Filaments { get; set; }
    }
}
