using System;
using System.Collections.Generic;

#nullable disable

namespace CircularSeasWebAPI.Entities
{
    public partial class Feature
    {
        public Feature()
        {
            FeatureMats = new HashSet<FeatureMat>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<FeatureMat> FeatureMats { get; set; }
    }
}
