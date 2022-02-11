using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Table("Feature")]
    public partial class Feature
    {
        public Feature()
        {
            FeatureMats = new HashSet<FeatureMat>();
        }

        [Key]
        public Guid ID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [InverseProperty(nameof(FeatureMat.FeatureFKNavigation))]
        public virtual ICollection<FeatureMat> FeatureMats { get; set; }
    }
}
