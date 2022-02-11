using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Table("Property")]
    public partial class Property
    {
        public Property()
        {
            PropMats = new HashSet<PropMat>();
        }

        [Key]
        public Guid ID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Unit { get; set; }
        public bool PositiveImpact { get; set; }

        [InverseProperty(nameof(PropMat.PropertyFKNavigation))]
        public virtual ICollection<PropMat> PropMats { get; set; }
    }
}
