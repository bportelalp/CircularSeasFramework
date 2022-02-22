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
                SpoolStock = row.Stocks?.FirstOrDefault()?.SpoolQuantity ?? 0,
                Deprecated = row.Deprecated,
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
                Deprecated = dom.Deprecated,
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

        internal static Models.Node Repo2Domain(Entities.Node row)
        {
            if (row == null) return null;
            Models.Node dom = new Models.Node()
            {
                Id = row.ID,
                Name = row.NodeName,
                IsProvider = row.IsProvider,
            };
            return dom;
        }

        internal static Entities.Node Domain2Repo(Models.Node dom)
        {
            if(dom == null) return null;
            Entities.Node row = new Entities.Node()
            {
                ID = dom.Id,
                NodeName = dom.Name,
                IsProvider = dom.IsProvider
            };
            return row;
        }

        internal static Models.Order Repo2Domain(Entities.Order row)
        {
            if(row == null) return null;
            Models.Order dom = new Models.Order()
            {
                Id = row.ID,
                NodeFK = row.NodeFK,
                ProviderFK = row.ProviderFK,
                MaterialFK = row.MaterialFK,
                Delivered = row.Delivered,
                DeliveryDate = row.DeliveryDate,
                CreationDate = row.CreationDate,
                SpoolQuantity = row.SpoolQuantity
            };
            return dom;
        }

        internal static Entities.Order Domain2Repo(Models.Order dom)
        {
            if (dom == null) return null;
            Entities.Order row = new Entities.Order()
            {
                ID = dom.Id,
                NodeFK = dom.NodeFK,
                ProviderFK = dom.ProviderFK,
                MaterialFK = dom.MaterialFK,
                Delivered = dom.Delivered,
                DeliveryDate = dom.DeliveryDate,
                CreationDate = dom.CreationDate,
                SpoolQuantity = dom.SpoolQuantity
            };
            return row;
        }



        internal static Models.Material MapMaterial(Entities.Material row)
        {
            if (row == null) return null;
            var dom = Mapper.Repo2Domain(row);
            dom.Evaluations = new List<Models.Evaluation>();
            foreach (var eval in row.PropMats)
            {
                var evaluation = Mapper.Repo2Domain(eval);
                evaluation.Property = Mapper.Repo2Domain(eval.PropertyFKNavigation);
                dom.Evaluations.Add(evaluation);

            }
            return dom;
        }

    }
}
