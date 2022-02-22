using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Table("Node")]
    public partial class Node
    {
        public Node()
        {
            OrderNodeFKNavigations = new HashSet<Order>();
            OrderProviderFKNavigations = new HashSet<Order>();
            Stocks = new HashSet<Stock>();
        }

        [Key]
        public Guid ID { get; set; }
        [Required]
        [StringLength(500)]
        public string NodeName { get; set; }
        public bool IsProvider { get; set; }

        [InverseProperty(nameof(Order.NodeFKNavigation))]
        public virtual ICollection<Order> OrderNodeFKNavigations { get; set; }
        [InverseProperty(nameof(Order.ProviderFKNavigation))]
        public virtual ICollection<Order> OrderProviderFKNavigations { get; set; }
        [InverseProperty(nameof(Stock.NodeFKNavigation))]
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
