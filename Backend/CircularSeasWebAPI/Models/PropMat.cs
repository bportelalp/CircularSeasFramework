using System;
using System.Collections.Generic;

#nullable disable

namespace CircularSeasWebAPI.Models
{
    public partial class PropMat
    {
        public int Id { get; set; }
        public int IdMaterial { get; set; }
        public int IdProperty { get; set; }
        public double Value { get; set; }

        public virtual Material IdMaterialNavigation { get; set; }
        public virtual Property IdPropertyNavigation { get; set; }
    }
}
