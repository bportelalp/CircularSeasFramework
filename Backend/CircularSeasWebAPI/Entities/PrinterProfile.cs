using System;
using System.Collections.Generic;

#nullable disable

namespace CircularSeasWebAPI.Entities
{
    public partial class PrinterProfile
    {
        public int Id { get; set; }
        public int PrinterId { get; set; }
        public string Profile { get; set; }

        public virtual Printer Printer { get; set; }
    }
}
