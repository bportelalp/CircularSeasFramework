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

        public async Task<List<Models.Material>> GetMaterials(bool includeProperties = true, Guid stockInNode = new Guid(), bool forUsers = true)
        {
            var response = new List<Models.Material>();

            var query = DbContext.Materials.AsNoTracking();
            if (includeProperties && !forUsers)
                query = query.Include(m => m.PropMats).ThenInclude(m => m.PropertyFKNavigation);
            else if (includeProperties && forUsers)
                query = query.Include(m => m.PropMats.Where(m => m.PropertyFKNavigation.Visible)).ThenInclude(m => m.PropertyFKNavigation);
            if (stockInNode == Guid.Empty)
                query = query.Include(m => m.Stocks);
            
            var mats = await query.ToListAsync();

            foreach (var entity in mats)
            {
                var material = Mapper.Repo2Domain(entity);
                material.Evaluations = new List<Models.Evaluation>();
                foreach (var eval in entity.PropMats)
                {
                    var evaluation = Mapper.Repo2Domain(eval);
                    evaluation.Property = Mapper.Repo2Domain(eval.PropertyFKNavigation);
                    material.Evaluations.Add(evaluation);
                    
                }
                response.Add(material);
            }
            
            return response;
        }
    }
}
