using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid NodeFK { get; set; }
        public Node Node { get; set; }
        public Guid ProviderFK { get; set; }
        public Guid MaterialFK { get; set; }
        public Material Material { get; set; }
        public int SpoolQuantity { get; set; }
        public DateTime CreationDate { get; set; }
        public bool Delivered { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}
