using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircularSeas.DB.Context;
using Microsoft.EntityFrameworkCore;

namespace CircularSeas.DB
{
    public class DbService
    {
        public DbService(CircularSeasContext dbContext)
        {
            DbContext = dbContext;
        }

        public CircularSeasContext DbContext { get; }


        public async Task<Models.Printer> GetPrinterInfo(string printerName)
        {
            var response = await DbContext.Printers.AsNoTracking()
                .Where(p => p.ModelName == printerName)
                .Include(p => p.PrinterProfiles)
                .FirstOrDefaultAsync();

            return Mapper.Repo2Domain(response);
        }

        public async Task<List<Models.Material>> GetMaterials(bool includeProperties = true)
        {
            var response = new List<Models.Material>();

            var qMats = DbContext.Materials.AsNoTracking();
            if (includeProperties)
                qMats = qMats.Include(m => m.PropMats).ThenInclude(m => m.PropertyFKNavigation);

            var mats = await qMats.ToListAsync();

            foreach (var entity in mats)
            {
                var material = Mapper.Repo2Domain(entity);
            }

            return null;
        }
    }
}
