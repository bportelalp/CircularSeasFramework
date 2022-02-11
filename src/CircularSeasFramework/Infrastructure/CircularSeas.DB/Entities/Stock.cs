using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Table("Stock")]
    public partial class Stock
    {
        [Key]
        public Guid ID { get; set; }
        public Guid NodeFK { get; set; }
        public Guid MaterialFK { get; set; }
        public int Quantity { get; set; }

        [ForeignKey(nameof(MaterialFK))]
        [InverseProperty(nameof(Material.Stocks))]
        public virtual Material MaterialFKNavigation { get; set; }
    }
}
