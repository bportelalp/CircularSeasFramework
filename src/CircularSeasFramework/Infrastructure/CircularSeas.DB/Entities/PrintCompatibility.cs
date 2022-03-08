using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Table("PrintCompatibility", Schema = "slicer")]
    public partial class PrintCompatibility
    {
        [Key]
        public Guid ID { get; set; }
        public Guid PrintFK { get; set; }
        public Guid PrinterFK { get; set; }

        [ForeignKey(nameof(PrintFK))]
        [InverseProperty(nameof(Print.PrintCompatibilities))]
        public virtual Print PrintFKNavigation { get; set; }
        [ForeignKey(nameof(PrinterFK))]
        [InverseProperty(nameof(Printer.PrintCompatibilities))]
        public virtual Printer PrinterFKNavigation { get; set; }
    }
}
