using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Table("Material")]
    public partial class Material
    {
        public Material()
        {
            PropMats = new HashSet<PropMat>();
            Stocks = new HashSet<Stock>();
        }

        [Key]
        public Guid ID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public double? BedTemperature { get; set; }
        public double? HotendTemperature { get; set; }
        public double? IdealTempExtr { get; set; }
        public double? MinTempExtr { get; set; }
        public double? MaxTempExtr { get; set; }

        [InverseProperty(nameof(PropMat.MaterialFKNavigation))]
        public virtual ICollection<PropMat> PropMats { get; set; }
        [InverseProperty(nameof(Stock.MaterialFKNavigation))]
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
