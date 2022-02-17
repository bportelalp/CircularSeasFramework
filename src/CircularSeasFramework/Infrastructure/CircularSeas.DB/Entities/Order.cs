﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Keyless]
    public partial class Order
    {
        public Guid ID { get; set; }
        public Guid NodeFK { get; set; }
        public Guid ProviderFK { get; set; }
        public Guid MaterialFK { get; set; }
        public int SpoolQuantity { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        public bool Delivered { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeliveryDate { get; set; }

        [ForeignKey(nameof(MaterialFK))]
        public virtual Material MaterialFKNavigation { get; set; }
        [ForeignKey(nameof(NodeFK))]
        public virtual Node NodeFKNavigation { get; set; }
        [ForeignKey(nameof(ProviderFK))]
        public virtual Node ProviderFKNavigation { get; set; }
    }
}
