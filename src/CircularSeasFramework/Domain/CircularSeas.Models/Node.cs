using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models
{
    public class Node
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsProvider { get; set; }
        public List<Order> Orders { get; set; }
    }
}
