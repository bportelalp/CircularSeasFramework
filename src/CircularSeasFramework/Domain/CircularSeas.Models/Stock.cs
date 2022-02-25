using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models
{
    public class Stock
    {
        public Guid Id { get; set; }
        public Guid MaterialFK { get; set; }
        public Guid NodeFK { get; set; }
        public int SpoolQuantity { get; set; }
        public Node Node { get; set; }
        public Material Material { get; set; }
    }
}
