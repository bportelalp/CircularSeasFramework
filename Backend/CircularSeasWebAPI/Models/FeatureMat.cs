using System;
using System.Collections.Generic;

#nullable disable

namespace CircularSeasWebAPI.Models
{
    public partial class FeatureMat
    {
        public int Id { get; set; }
        public int IdMaterial { get; set; }
        public int IdFeature { get; set; }
        public bool Value { get; set; }

        public virtual Feature IdFeatureNavigation { get; set; }
        public virtual Material IdMaterialNavigation { get; set; }
    }
}
