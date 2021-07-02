using System;
using System.Collections.Generic;

#nullable disable

namespace CircularSeasWebAPI.Models
{
    public partial class Material
    {
        public Material()
        {
            FeatureMats = new HashSet<FeatureMat>();
            PropMats = new HashSet<PropMat>();
            Stocks = new HashSet<Stock>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<FeatureMat> FeatureMats { get; set; }
        public virtual ICollection<PropMat> PropMats { get; set; }
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
