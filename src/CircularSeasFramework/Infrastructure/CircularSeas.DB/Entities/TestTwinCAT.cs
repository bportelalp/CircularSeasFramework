using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CircularSeas.DB.Entities
{
    [Table("TestTwinCAT")]
    public partial class TestTwinCAT
    {
        [Key]
        public int ID { get; set; }
        public double? Value { get; set; }
    }
}
