using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.Infrastructure.DB.Entities
{
    [Table("Prints", Schema = "slicer")]
    public partial class Print
    {
        public Print()
        {
            FilamentCompatibilities = new HashSet<FilamentCompatibility>();
            PrintCompatibilities = new HashSet<PrintCompatibility>();
            PrintSettings = new HashSet<PrintSetting>();
        }

        [Key]
        public Guid ID { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string iniKeyword { get; set; }

        [InverseProperty(nameof(FilamentCompatibility.PrintFKNavigation))]
        public virtual ICollection<FilamentCompatibility> FilamentCompatibilities { get; set; }
        [InverseProperty(nameof(PrintCompatibility.PrintFKNavigation))]
        public virtual ICollection<PrintCompatibility> PrintCompatibilities { get; set; }
        [InverseProperty(nameof(PrintSetting.PrintFKNavigation))]
        public virtual ICollection<PrintSetting> PrintSettings { get; set; }
    }
}
