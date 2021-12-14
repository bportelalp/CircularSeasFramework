using System;
using System.Collections.Generic;

#nullable disable

namespace CircularSeasWebAPI.Entities
{
    public partial class Stock
    {
        public int Id { get; set; }
        public int IdNode { get; set; }
        public int IdMaterial { get; set; }
        public int Quantity { get; set; }

        public virtual Material IdMaterialNavigation { get; set; }
    }
}
