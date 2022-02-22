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
        public bool? MoreIsBetter { get; set; }
        public string HelpText { get; set; }
        public bool Visible { get; set; }

        public List<string> DichotomousOptions => DichotomousOptionsList;
        public List<string> MoreIsBetterOptions => MoreIsBetterOptionsList;
        //Helper options
        public static List<string> DichotomousOptionsList = new List<string>() { "Yes", "No"};
        public static List<string> MoreIsBetterOptionsList = new List<string>() { "Yes", "No", "Does not apply" };
        public string IsDichotomousStr
        {
            get { return  (IsDichotomous == true ? "Yes" : "No"); }
            set { IsDichotomous = (value == "Yes" ? true : false); }
        }
        public string MoreIsBetterStr
        {
            get { return MoreIsBetter == null ? "Does not apply" : (MoreIsBetter == true ? "Yes" : "No"); }
            set { MoreIsBetter = value == "Does not apply" ? null : (value == "Yes" ? (bool?)true : (bool?)false); }
        }
    }

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
        public static List<string> BinOptionsList = new List<string>() { "Yes", "No"};
    }
}
