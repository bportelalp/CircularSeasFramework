using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models
{
    public class Property
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDichotomous { get; set; }
        public string Unit { get; set; }
        public bool MoreIsBetter { get; set; }
        public string HelpText  { get; set; }
        public bool Visible { get; set; }
    }

    public class Evaluation
    {
        public Guid Id { get; set; }
        public Property Property { get; set; }
        public double? ValueDec  { get; set; } = null;
        public bool? ValueBin { get; set; } = null;
    }
}
