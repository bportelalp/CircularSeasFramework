using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Table("PrinterProfile")]
    public partial class PrinterProfile
    {
        [Key]
        public Guid ID { get; set; }
        public Guid PrinterFK { get; set; }
        [Required]
        [StringLength(200)]
        public string ProfileName { get; set; }

        [ForeignKey(nameof(PrinterFK))]
        [InverseProperty(nameof(Printer.PrinterProfiles))]
        public virtual Printer PrinterFKNavigation { get; set; }
    }
}
