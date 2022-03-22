using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.Infrastructure.DB.Entities
{
    [Table("PrintSettings", Schema = "slicer")]
    public partial class PrintSetting
    {
        [Key]
        public Guid ID { get; set; }
        public Guid PrintFK { get; set; }
        [Required]
        [StringLength(100)]
        public string iniKey { get; set; }
        [StringLength(2000)]
        public string iniValue { get; set; }

        [ForeignKey(nameof(PrintFK))]
        [InverseProperty(nameof(Print.PrintSettings))]
        public virtual Print PrintFKNavigation { get; set; }
    }
}
