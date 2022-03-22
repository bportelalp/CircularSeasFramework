using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.Infrastructure.DB.Entities
{
    [Table("PropMat")]
    public partial class PropMat
    {
        [Key]
        public Guid ID { get; set; }
        public Guid MaterialFK { get; set; }
        public Guid PropertyFK { get; set; }
        public double? ValueDec { get; set; }
        public bool? ValueBin { get; set; }

        [ForeignKey(nameof(MaterialFK))]
        [InverseProperty(nameof(Material.PropMats))]
        public virtual Material MaterialFKNavigation { get; set; }
        [ForeignKey(nameof(PropertyFK))]
        [InverseProperty(nameof(Property.PropMats))]
        public virtual Property PropertyFKNavigation { get; set; }
    }
}
