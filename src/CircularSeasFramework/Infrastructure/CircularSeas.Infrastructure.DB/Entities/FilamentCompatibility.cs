using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.Infrastructure.DB.Entities
{
    [Table("FilamentCompatibility", Schema = "slicer")]
    public partial class FilamentCompatibility
    {
        [Key]
        public Guid ID { get; set; }
        public Guid FilamentFK { get; set; }
        public Guid? PrintFK { get; set; }
        public Guid? PrinterFK { get; set; }

        [ForeignKey(nameof(FilamentFK))]
        [InverseProperty(nameof(Filament.FilamentCompatibilities))]
        public virtual Filament FilamentFKNavigation { get; set; }
        [ForeignKey(nameof(PrintFK))]
        [InverseProperty(nameof(Print.FilamentCompatibilities))]
        public virtual Print PrintFKNavigation { get; set; }
        [ForeignKey(nameof(PrinterFK))]
        [InverseProperty(nameof(Printer.FilamentCompatibilities))]
        public virtual Printer PrinterFKNavigation { get; set; }
    }
}
