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
            if (stockInNode != Guid.Empty)
                query = query.Include(m => m.Stocks.Where(s => s.NodeFK == stockInNode));

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
                material.Stock = Mapper.Repo2Domain(entity.Stocks.FirstOrDefault());
                response.Add(material);
            }
            return response;
        }

        public async Task<Models.Material> GetMaterialDetail(Guid id)
        {
            var response = new Models.Material();
            var query = DbContext.Materials.AsNoTracking()
                .Where(m => m.ID == id)
                .Include(m => m.PropMats)
                .ThenInclude(m => m.PropertyFKNavigation);

            var mat = await query.FirstOrDefaultAsync();

            response = Mapper.MapMaterial(mat);
            return response;
        }

        public async Task<List<Models.Property>> GetProperties()
        {
            var response = new List<Models.Property>();
            var props = await DbContext.Properties.AsNoTracking().ToListAsync();
            foreach (var prop in props)
            {
                response.Add(Mapper.Repo2Domain(prop));
            }
            return response;
        }

        public async Task<Models.Property> GetPropertyDetail(Guid id)
        {
            var response = new Models.Property();
            var prop = await DbContext.Properties.AsNoTracking()
                .Where(p => p.ID == id)
                .FirstOrDefaultAsync();
            response = Mapper.Repo2Domain(prop);
            return response;
        }

        public async Task<List<Models.Node>> GetNodes()
        {
            var response = new List<Models.Node>();
            var nodes = await DbContext.Nodes.AsNoTracking().ToListAsync();

            foreach (var node in nodes)
            {
                response.Add(Mapper.Repo2Domain(node));
            }
            return response;
        }
        public async Task<Models.Material> GetMaterialSchema()
        {
            var response = new Models.Material();
            response.Evaluations = new List<Models.Evaluation>();
            var properties = await DbContext.Properties.AsNoTracking().ToListAsync();
            foreach (var prop in properties)
            {
                response.Evaluations.Add(new Models.Evaluation() { Property = Mapper.Repo2Domain(prop) });
            }
            return response;
        }

        public async Task<List<Models.Order>> GetOrders(int status = 0, Guid specificNode = default(Guid), bool includeMaterial = true)
        {
            var response = new List<Models.Order>();

            var query = DbContext.Orders.AsNoTracking();
            if (status == 1)
                query = query.Where(o => o.ShippingDate == null);
            else if (status == 2)
                query = query.Where(o => o.ShippingDate != null && o.FinishedDate == null);
            else if (status == 3)
                query = query.Where(o => o.FinishedDate != null);
            else if (status == 0)
                query = query.Where(o => o.FinishedDate == null);

            if (specificNode != default(Guid))
                query = query.Where(o => o.NodeFK == specificNode);
            if (includeMaterial)
                query = query.Include(o => o.MaterialFKNavigation);

            var orders = await query
                .Include(o => o.NodeFKNavigation)
                .ToListAsync();
            foreach (var order in orders)
            {
                var dom = Mapper.Repo2Domain(order);
                dom.Material = Mapper.Repo2Domain(order.MaterialFKNavigation);
                dom.Node = Mapper.Repo2Domain(order.NodeFKNavigation);
                response.Add(dom);
            }

            return response;
        }

        public async Task<Models.Order> GetOrder(Guid orderId)
        {
            var response = new Models.Order();
            var order = await DbContext.Orders.AsNoTracking()
                .Where(o => o.ID == orderId)
                .Include(o => o.NodeFKNavigation)
                .Include(o => o.MaterialFKNavigation)
                .FirstOrDefaultAsync();

            response = Mapper.Repo2Domain(order);
            response.Material = Mapper.Repo2Domain(order.MaterialFKNavigation);
            response.Node = Mapper.Repo2Domain(order.NodeFKNavigation);

            return response;
        }

        public async Task<List<Models.Material>> CheckBadMaterialsVisible(Guid propertyId)
        {
            var response = new List<Models.Material>();
            var property = await this.GetPropertyDetail(propertyId);
            var materials = await DbContext.Materials.AsNoTracking()
                .Include(m => m.PropMats)
                .ThenInclude(m => m.PropertyFKNavigation)
                .ToListAsync();
            materials = materials
                .Where(m => !DbHelpers.PropertyFilled(m, propertyId))
                .ToList();
            foreach (var mat in materials)
            {
                response.Add(Mapper.MapMaterial(mat));
            }
            return response;
        }

        public async Task UpdateMaterial(Models.Material material)
        {
            if (material == null) return;
            var rowMat = Mapper.Domain2Repo(material);

            DbContext.Update(rowMat);
            await UpdateMaterialEvaluations(material);
        }

        public async Task UpdateProperty(Models.Property property)
        {
            if (property == null) return;
            var rowProp = Mapper.Domain2Repo(property);

            DbContext.Update(rowProp);
            await DbContext.SaveChangesAsync();
        }

        public async Task<Models.Order> UpdateOrder(Models.Order order)
        {
            if (order == null) return null;
            var rowOrder = Mapper.Domain2Repo(order);

            DbContext.Update(rowOrder);
            await DbContext.SaveChangesAsync();
            return order;
        }

        public async Task<Models.Stock> UpdateStock(Models.Order order)
        {
            if (order == null) return null;

            var stock = await DbContext.Stocks.AsNoTracking()
                .Where(s => s.NodeFK == order.NodeFK && s.MaterialFK == order.MaterialFK)
                .FirstOrDefaultAsync();
            if (stock == null)
            {
                stock = new Entities.Stock()
                {
                    ID = Guid.NewGuid(),
                    NodeFK = order.NodeFK,
                    MaterialFK = order.MaterialFK,
                    SpoolQuantity = order.SpoolQuantity,
                };
                DbContext.Add(stock);
            }
            else
            {
                stock.SpoolQuantity = order.SpoolQuantity + stock.SpoolQuantity;
                DbContext.Update(stock);
            }

            await DbContext.SaveChangesAsync();

            var stk = Mapper.Repo2Domain(stock);
            stk.Material = order.Material;
            stk.Node = order.Node;
            return stk;
        }

        public async Task<Models.Stock> UpdateStock(Guid nodeId, Guid materialId, int amount)
        {
            var stock = await DbContext.Stocks.AsNoTracking()
                .Where(s => s.MaterialFK == materialId)
                .Where(s => s.NodeFK == nodeId)
                .FirstOrDefaultAsync();

            if(stock == null) return null;

            if (stock.SpoolQuantity >= amount)
                stock.SpoolQuantity = stock.SpoolQuantity - amount;

            DbContext.Update(stock);
            await DbContext.SaveChangesAsync();

            var stk = Mapper.Repo2Domain(stock);
            return stk;
        }

        public async Task UpdateMaterialEvaluations(Models.Material material)
        {
            List<Entities.PropMat> propmats = new List<Entities.PropMat>();
            foreach (var eval in material.Evaluations)
            {
                propmats.Add(new Entities.PropMat()
                {
                    ID = eval.Id,
                    MaterialFK = material.Id,
                    PropertyFK = eval.Property.Id,
                    ValueBin = eval.ValueBin,
                    ValueDec = eval.ValueDec
                });
            }
            DbContext.UpdateRange(propmats);
            await DbContext.SaveChangesAsync();
        }

        public async Task UpdateVisibility(Guid id, bool visible)
        {
            var property = await DbContext.Properties.Where(p => p.ID == id).FirstOrDefaultAsync();
            property.Visible = visible;
            DbContext.Update(property);
            await DbContext.SaveChangesAsync();
        }

        public async Task<Models.Property> CreateProperty(Models.Property property)
        {
            property.Id = Guid.NewGuid();

            List<Guid> materialsID = await DbContext.Materials.AsNoTracking().Select(m => m.ID).ToListAsync();
            Entities.Property row = Mapper.Domain2Repo(property);
            List<Entities.PropMat> propMats = new List<Entities.PropMat>();

            foreach (var mat in materialsID)
            {
                propMats.Add(new Entities.PropMat()
                {
                    ID = Guid.NewGuid(),
                    MaterialFK = mat,
                    PropertyFK = property.Id,
                    ValueBin = null,
                    ValueDec = null
                });
            }

            DbContext.Add(row);
            DbContext.AddRange(propMats);
            await DbContext.SaveChangesAsync();
            return Mapper.Repo2Domain(row);
        }

        public async Task<Models.Material> CreateMaterial(Models.Material material)
        {
            material.Id = Guid.NewGuid();
            var rowMat = Mapper.Domain2Repo(material);

            List<Entities.PropMat> propmats = new List<Entities.PropMat>();
            foreach (var eval in material.Evaluations)
            {
                eval.Id = Guid.NewGuid();
                propmats.Add(new Entities.PropMat()
                {
                    ID = eval.Id,
                    MaterialFK = material.Id,
                    PropertyFK = eval.Property.Id,
                    ValueBin = eval.ValueBin,
                    ValueDec = eval.ValueDec
                });
            }
            DbContext.Add(rowMat);
            DbContext.AddRange(propmats);
            await DbContext.SaveChangesAsync();

            return material;

        }

        public async Task<Models.Order> CreateOrder(Models.Order order)
        {
            order.Id = Guid.NewGuid();
            var rowOrd = Mapper.Domain2Repo(order);

            DbContext.Add(rowOrd);
            await DbContext.SaveChangesAsync();
            return order;
        }

        public async Task DeleteProperty(Guid id)
        {
            var property = new Entities.Property() { ID = id };
            var propMats = await DbContext.PropMats.Where(p => p.PropertyFK == id).ToListAsync();

            DbContext.RemoveRange(propMats);
            DbContext.Remove(property);
            DbContext.SaveChanges();
        }

        public async Task DeleteMaterial(Guid id)
        {
            var material = new Entities.Material() { ID = id };
            var propMats = await DbContext.PropMats.Where(p => p.MaterialFK == id).ToListAsync();

            DbContext.RemoveRange(propMats);
            DbContext.Remove(material);
            DbContext.SaveChanges();
        }
    }

    internal static class DbHelpers
    {
        internal static bool PropertyFilled(Entities.Material mat, Guid propId)
        {
            var eval = mat.PropMats.Where(p => p.PropertyFK == propId).FirstOrDefault();
            if (eval == null)
                return false;
            else if (eval.ValueBin == null && eval.ValueDec == null)
                return false;
            else
                return true;
        }
    }
}
