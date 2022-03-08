using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Table("FilamentSettings", Schema = "slicer")]
    public partial class FilamentSetting
    {
        [Key]
        public Guid ID { get; set; }
        public Guid FilamentFK { get; set; }
        [Required]
        [StringLength(100)]
        public string iniKey { get; set; }
        [StringLength(2000)]
        public string iniValue { get; set; }

        [ForeignKey(nameof(FilamentFK))]
        [InverseProperty(nameof(Filament.FilamentSettings))]
        public virtual Filament FilamentFKNavigation { get; set; }
    }
}
