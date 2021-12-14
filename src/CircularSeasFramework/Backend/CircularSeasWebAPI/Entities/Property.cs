using System;
using System.Collections.Generic;

#nullable disable

namespace CircularSeasWebAPI.Entities
{
    public partial class Property
    {
        public Property()
        {
            PropMats = new HashSet<PropMat>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Units { get; set; }
        public bool PositiveImpact { get; set; }

        public virtual ICollection<PropMat> PropMats { get; set; }
    }
}
