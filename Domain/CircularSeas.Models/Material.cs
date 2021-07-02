using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models
{
    public class Material
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public double[] propiedades { get; set; }
        public int stock { get; set; }
    }
}
