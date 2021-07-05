using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircularSeasWebAPI.Helpers
{
    internal static class Mapper
    {
        internal static CircularSeas.Models.Printer Repo2Domain(Entities.Printer _printer)
        {
            CircularSeas.Models.Printer dom = new CircularSeas.Models.Printer();
            dom.Name = _printer.Name;
            dom.FilamentDiameter = _printer.FilamentDiameter;
            List<string> profiles = new List<string>();
            _printer.PrinterProfiles.ToList().ForEach(pr => profiles.Add(pr.Profile));
            dom.Profiles = profiles.ToArray();
            return dom;
        }

        internal static List<CircularSeas.Models.Filament> Repo2Domain(List<Entities.Material> _material)
        {
            List<CircularSeas.Models.Filament> dom = new List<CircularSeas.Models.Filament>();

            foreach (var item in _material)
            {
                List<bool> featuresValues = new List<bool>();
                List<double> propertiesValues = new List<double>();
                item.FeatureMats.ToList().ForEach(fm => featuresValues.Add(fm.Value));
                item.PropMats.ToList().ForEach(pm => propertiesValues.Add(pm.Value));

                dom.Add(new CircularSeas.Models.Filament
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
    }
}
