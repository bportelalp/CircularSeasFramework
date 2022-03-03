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
            Orders = new HashSet<Order>();
            PropMats = new HashSet<PropMat>();
            Stocks = new HashSet<Stock>();
        }

        [Key]
        public Guid ID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Deprecated { get; set; }

        [InverseProperty(nameof(Order.MaterialFKNavigation))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(PropMat.MaterialFKNavigation))]
        public virtual ICollection<PropMat> PropMats { get; set; }
        [InverseProperty(nameof(Stock.MaterialFKNavigation))]
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
