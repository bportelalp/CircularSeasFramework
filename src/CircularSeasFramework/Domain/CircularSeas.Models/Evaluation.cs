using System;
using System.Collections.Generic;
using System.Text;

namespace CircularSeas.Models
{
    public class Evaluation
    {
        public Guid Id { get; set; }
        public Property Property { get; set; }
        public double? ValueDec { get; set; } = null;
        public bool? ValueBin { get; set; } = null;

        //Helper properties
        public string ValueStr => Property.IsDichotomous ? ValueBinStr : Convert.ToString(ValueDec);
        public string ValueBinStr
        {
            get { return ValueBin == null ? "Does not apply" : (ValueBin == true ? "Yes" : "No"); }
            set { ValueBin = value == "Does not apply" ? null : (value == "Yes" ? (bool?)true : (bool?)false); }
        }

        public List<string> BinOptions => BinOptionsList;
        public static List<string> BinOptionsList = new List<string>() { "Yes", "No" };
    }
}
