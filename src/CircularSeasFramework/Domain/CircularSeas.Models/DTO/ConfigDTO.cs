using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace CircularSeas.Models.DTO
{
    public class ConfigDTO
    {
        public Dictionary<string, Guid> Matching { get; set; }
        public List<string> Lines { get; set; }
    }
}
