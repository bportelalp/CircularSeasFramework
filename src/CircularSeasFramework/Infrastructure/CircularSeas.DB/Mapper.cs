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
            Models.Printer dom = new Models.Printer()
            {
                Id = row.ID,
                Name = row.ModelName,
                FilamentDiameter = row.FilamentDiameter

            };
            row.PrinterProfiles.ToList().ForEach(pr => dom.Profiles.Add(pr.ProfileName));
            return dom;
        }

        internal static Entities.Printer Domain2Repo(Models.Printer printer)
        {
            Entities.Printer row = new Entities.Printer()
            {
                ID = printer.Id,
                ModelName = printer.Name,
                FilamentDiameter = printer.FilamentDiameter
                //Ignore printer profiles by now
            };
            return row;
        }

        internal static List<Models.Material> Repo2Domain(List<Entities.Material> _material)
        {
            List<Models.Material> dom = new List<Models.Material>();

            foreach (var item in _material)
            {
                List<bool> featuresValues = new List<bool>();
                List<double> propertiesValues = new List<double>();
                //item.PropMats.ToList().ForEach(pm => propertiesValues.Add(pm.ValueDec));

                dom.Add(new CircularSeas.Models.Material
                {
                    Name = item.Name,
                    Description = item.Description,
                    FeaturesValues = featuresValues.ToArray(),
                    PropertiesValues = propertiesValues.ToArray(),
                    SpoolStock = 0
                });
            }

            return dom;
        }
        internal static Models.Material Repo2Domain(Entities.Material row)
        {
            Models.Material dom = new Models.Material()
            {
                Id = row.ID,
                Name = row.Name,
                Description = row.Description,
                BedTemperature = row.BedTemperature,
                HotendTemperature = row.HotendTemperature,
                IdealTempExtr = row.IdealTempExtr,
                MinTempExtr = row.MinTempExtr,
                MaxTempExtr = row.MaxTempExtr
                //TODO: Spool Stock
            };
            return dom;
        }

        internal static Entities.Material Domain2Repo(Models.Material material)
        {
            Entities.Material row = new Entities.Material()
            {
                ID = material.Id,
                Name = material.Name,
                Description= material.Description,
                BedTemperature= material.BedTemperature,
                HotendTemperature= material.HotendTemperature,
                IdealTempExtr = material.IdealTempExtr,
                MinTempExtr = material.MinTempExtr,
                MaxTempExtr = material.MaxTempExtr
            };
            return row;
        }

        internal static Models.Property Repo2Domain(Entities.Property row)
        {
            Models.Property dom = new Models.Property()
            {
                Id = row.ID,
                Name = row.Name,
                HelpText = row.HelpText,
                IsDichotomous = row.IsDichotomous,
                MoreIsBetter = row.MoreIsBetter,
                Unit = row.Unit,
            };
            return dom;
        }

        internal static Entities.Property Domain2Repo(Models.Property property)
        {
            Entities.Property row = new Entities.Property()
            {
                ID = property.Id,
                Name = property.Name,
                HelpText = property.HelpText,
                IsDichotomous = property.IsDichotomous,
                MoreIsBetter = property.MoreIsBetter,
                Unit = property.Unit,
            };
            return row;
        }
    }
}
