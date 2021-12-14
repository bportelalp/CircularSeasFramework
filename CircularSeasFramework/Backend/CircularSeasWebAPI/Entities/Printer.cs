using System;
using System.Collections.Generic;

#nullable disable

namespace CircularSeasWebAPI.Entities
{
    public partial class Printer
    {
        public Printer()
        {
            PrinterProfiles = new HashSet<PrinterProfile>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double FilamentDiameter { get; set; }

        public virtual ICollection<PrinterProfile> PrinterProfiles { get; set; }
    }
}
