using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models.DTO
{
    public class QrDTO
    {
        public Guid OrderId { get; set; }
        public Guid MaterialId { get; set; }
        public string MaterialName { get; set; }
    }
}
