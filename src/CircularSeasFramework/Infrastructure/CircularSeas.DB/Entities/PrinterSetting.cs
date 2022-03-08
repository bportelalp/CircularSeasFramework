using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Table("PrinterSettings", Schema = "slicer")]
    public partial class PrinterSetting
    {
        [Key]
        public Guid ID { get; set; }
        public Guid PrinterFK { get; set; }
        [Required]
        [StringLength(100)]
        public string iniKey { get; set; }
        [StringLength(2000)]
        public string iniValue { get; set; }

        [ForeignKey(nameof(PrinterFK))]
        [InverseProperty(nameof(Printer.PrinterSettings))]
        public virtual Printer PrinterFKNavigation { get; set; }
    }
}
