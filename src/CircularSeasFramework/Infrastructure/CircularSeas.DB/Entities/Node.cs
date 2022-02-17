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
            Stocks = new HashSet<Stock>();
        }

        [Key]
        public Guid ID { get; set; }
        [Required]
        [StringLength(500)]
        public string NodeName { get; set; }
        public bool IsProvider { get; set; }

        [InverseProperty(nameof(Stock.NodeFKNavigation))]
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
