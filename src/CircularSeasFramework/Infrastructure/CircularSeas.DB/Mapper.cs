using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircularSeas.DB
{
    internal class Mapper
    {
        internal static Models.Printer Repo2Domain(Entities.Printer row)
        {
            if (row == null) return null;
            Models.Printer dom = new Models.Printer()
            {
                Id = row.ID,
                Name = row.ModelName,
                FilamentDiameter = row.FilamentDiameter

            };
            row.PrinterProfiles.ToList().ForEach(pr => dom.Profiles.Add(pr.ProfileName));
            return dom;
        }

        internal static Entities.Printer Domain2Repo(Models.Printer dom)
        {
            if (dom == null) return null;
            Entities.Printer row = new Entities.Printer()
            {
                ID = dom.Id,
                ModelName = dom.Name,
                FilamentDiameter = dom.FilamentDiameter
                //Ignore printer profiles by now
            };
            return row;
        }

        internal static Models.Material Repo2Domain(Entities.Material row)
        {
            if (row == null) return null;
            Models.Material dom = new Models.Material()
            {
                Id = row.ID,
                Name = row.Name,
                Description = row.Description,
                BedTemperature = row.BedTemperature,
                HotendTemperature = row.HotendTemperature,
                IdealTempExtr = row.IdealTempExtr,
                MinTempExtr = row.MinTempExtr,
                MaxTempExtr = row.MaxTempExtr,
                SpoolStock = row.Stocks?.FirstOrDefault()?.SpoolQuantity ?? 0,
            };
            return dom;
        }

        internal static Entities.Material Domain2Repo(Models.Material dom)
        {
            if (dom == null) return null;
            Entities.Material row = new Entities.Material()
            {
                ID = dom.Id,
                Name = dom.Name,
                Description = dom.Description,
                BedTemperature = dom.BedTemperature,
                HotendTemperature = dom.HotendTemperature,
                IdealTempExtr = dom.IdealTempExtr,
                MinTempExtr = dom.MinTempExtr,
                MaxTempExtr = dom.MaxTempExtr
            };
            return row;
        }

        internal static Models.Evaluation Repo2Domain(Entities.PropMat row)
        {
            if (row == null) return null;
            Models.Evaluation dom = new Models.Evaluation()
            {
                Id = row.ID,
                ValueBin = row.ValueBin,
                ValueDec = row.ValueDec,
            };
            return dom;
        }
        internal static Entities.PropMat Domain2Repo(Models.Evaluation dom)
        {
            if (dom == null) return null;
            Entities.PropMat row = new Entities.PropMat()
            {
                ID = dom.Id,
                ValueBin = dom.ValueBin,
                ValueDec = dom.ValueDec,
            };
            return row;
        }
        internal static Models.Property Repo2Domain(Entities.Property row)
        {
            if (row == null) return null;
            Models.Property dom = new Models.Property()
            {
                Id = row.ID,
                Name = row.Name,
                HelpText = row.HelpText,
                IsDichotomous = row.IsDichotomous,
                MoreIsBetter = row.MoreIsBetter,
                Unit = row.Unit,
                Visible = row.Visible,
            };
            return dom;
        }

        internal static Entities.Property Domain2Repo(Models.Property dom)
        {
            if (dom == null) return null;
            Entities.Property row = new Entities.Property()
            {
                ID = dom.Id,
                Name = dom.Name,
                HelpText = dom.HelpText,
                IsDichotomous = dom.IsDichotomous,
                MoreIsBetter = dom.MoreIsBetter,
                Unit = dom.Unit,
                Visible = dom.Visible,
            };
            return row;
        }
    }
}
