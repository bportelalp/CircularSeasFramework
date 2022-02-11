using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Table("FeatureMat")]
    public partial class FeatureMat
    {
        [Key]
        public Guid ID { get; set; }
        public Guid MaterialFK { get; set; }
        public Guid FeatureFK { get; set; }
        public bool Value { get; set; }

        [ForeignKey(nameof(FeatureFK))]
        [InverseProperty(nameof(Feature.FeatureMats))]
        public virtual Feature FeatureFKNavigation { get; set; }
        [ForeignKey(nameof(MaterialFK))]
        [InverseProperty(nameof(Material.FeatureMats))]
        public virtual Material MaterialFKNavigation { get; set; }
    }
}
