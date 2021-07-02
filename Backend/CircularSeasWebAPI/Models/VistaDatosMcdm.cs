using System;
using System.Collections.Generic;

#nullable disable

namespace CircularSeasWebAPI.Models
{
    public partial class VistaDatosMcdm
    {
        public string Name { get; set; }
        public string Prop { get; set; }
        public bool PositiveImpact { get; set; }
        public double Value { get; set; }
        public string Units { get; set; }
    }
}
