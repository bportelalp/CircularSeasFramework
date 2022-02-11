using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Table("Printer")]
    public partial class Printer
    {
        public Printer()
        {
            PrinterProfiles = new HashSet<PrinterProfile>();
        }

        [Key]
        public Guid ID { get; set; }
        [Required]
        [StringLength(50)]
        public string ModelName { get; set; }
        public double FilamentDiameter { get; set; }

        [InverseProperty(nameof(PrinterProfile.PrinterFKNavigation))]
        public virtual ICollection<PrinterProfile> PrinterProfiles { get; set; }
    }
}
