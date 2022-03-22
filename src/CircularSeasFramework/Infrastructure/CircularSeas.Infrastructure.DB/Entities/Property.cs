using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.Infrastructure.DB.Entities
{
    public partial class Property
    {
        public Property()
        {
            PropMats = new HashSet<PropMat>();
        }

        [Key]
        public Guid ID { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public bool IsDichotomous { get; set; }
        [StringLength(10)]
        public string Unit { get; set; }
        public bool? MoreIsBetter { get; set; }
        [StringLength(500)]
        public string HelpText { get; set; }
        public bool Visible { get; set; }

        [InverseProperty(nameof(PropMat.PropertyFKNavigation))]
        public virtual ICollection<PropMat> PropMats { get; set; }
    }
}
