using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models
{
    public class Material
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //Data comes in array because they´re identified by position according to InfoTopsis data
        public bool Deprecated { get; set; }
        public List<Evaluation> Evaluations {get;set;}
        public Models.Stock Stock { get; set; }
        public string SpoolQuantity => (Stock?.SpoolQuantity > 0) ? (Stock.SpoolQuantity + $" In Stock") : "Out Stock";
    }
}
