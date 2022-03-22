using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.Infrastructure.DB.Entities
{
    [Table("Printers", Schema = "slicer")]
    public partial class Printer
    {
        public Printer()
        {
            FilamentCompatibilities = new HashSet<FilamentCompatibility>();
            PrintCompatibilities = new HashSet<PrintCompatibility>();
            PrinterSettings = new HashSet<PrinterSetting>();
        }

        [Key]
        public Guid ID { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string iniKeyword { get; set; }

        [InverseProperty(nameof(FilamentCompatibility.PrinterFKNavigation))]
        public virtual ICollection<FilamentCompatibility> FilamentCompatibilities { get; set; }
        [InverseProperty(nameof(PrintCompatibility.PrinterFKNavigation))]
        public virtual ICollection<PrintCompatibility> PrintCompatibilities { get; set; }
        [InverseProperty(nameof(PrinterSetting.PrinterFKNavigation))]
        public virtual ICollection<PrinterSetting> PrinterSettings { get; set; }
    }
}
