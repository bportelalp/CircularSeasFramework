using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Table("Filaments", Schema = "slicer")]
    public partial class Filament
    {
        public Filament()
        {
            FilamentCompatibilities = new HashSet<FilamentCompatibility>();
            FilamentSettings = new HashSet<FilamentSetting>();
        }

        [Key]
        public Guid ID { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string iniKeyword { get; set; }
        public Guid? MaterialFK { get; set; }

        [ForeignKey(nameof(MaterialFK))]
        [InverseProperty(nameof(Material.Filaments))]
        public virtual Material MaterialFKNavigation { get; set; }
        [InverseProperty(nameof(FilamentCompatibility.FilamentFKNavigation))]
        public virtual ICollection<FilamentCompatibility> FilamentCompatibilities { get; set; }
        [InverseProperty(nameof(FilamentSetting.FilamentFKNavigation))]
        public virtual ICollection<FilamentSetting> FilamentSettings { get; set; }
    }
}
