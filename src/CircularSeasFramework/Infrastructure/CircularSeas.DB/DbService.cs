using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using CircularSeas.DB.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace CircularSeas.DB
{
    public class DbService
    {
        public DbService(CircularSeasContext dbContext)
        {
            DbContext = dbContext;
        }

        public CircularSeasContext DbContext { get; }

        #region READ
        public async Task<Models.DTO.PrintDTO> GetPrintDTO(string printerCompatible, Guid stockInNode) //TODO
        {
            var response = new Models.DTO.PrintDTO();

            //Preliminary

            //ID de la impresora que se solicita
            var printerId = (await DbContext.Printers.AsNoTracking().FirstOrDefaultAsync(p => p.Name == printerCompatible)).ID;

            //Todos los filamentos compatibles con esa impresora, incluido el material asociado
            //  Descartar aquellos que no tienen material asociado
            var filCompatible = await DbContext.Filaments.AsNoTracking()
                .Include(f => f.FilamentCompatibilities)
                .Include(f => f.MaterialFKNavigation)
                .Where(f => f.FilamentCompatibilities.Select(fc => fc.PrinterFK).Contains(printerId) && f.MaterialFK != null)
                .ToListAsync();
            //Lista de ids de los filamentos compatibles
            var filCompatibleIDs = filCompatible.Select(fc => fc.ID).ToList();
            //Lista de Ids de los materiales cuyo filamento sirve para la impresora
            var materialIDs = filCompatible.Select(f => f.MaterialFK).ToList();
            
            // Conseguir dichos materiales para enviarlos.
            response.Materials = new List<Models.Material>();
            var mats = await DbContext.Materials.AsNoTracking()
                .Include(m => m.Stocks.Where(s => s.NodeFK == stockInNode))
                .Include(m => m.PropMats.Where(m => m.PropertyFKNavigation.Visible)).ThenInclude(m => m.PropertyFKNavigation)
                .Where(m => materialIDs.Contains(m.ID))
                .ToListAsync();

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
                response.Materials.Add(material);
            }

            //Prints compatibles con la impresora. Coger de la tabla PrintCompatibilities aquellos cuya PrinterFK coincide con la en cuestion e incluir los prints
            var printsCompPrinter = await DbContext.PrintCompatibilities.AsNoTracking()
                .Include(pc => pc.PrintFKNavigation)
                .Where(pc => pc.PrinterFK == printerId)
                .ToListAsync();

            var printsCompPrinterIDs = printsCompPrinter.Select(pc => pc.PrintFK).ToList();

            //prints compatibles con cada filamento compatible con la impresora (los de la lista filCompatible)
            //  Que sea compatibilidad con PrintFK y no con PrinterFK
            //  Que esté relacionado con un filamento ya detectado como compatible en filCompatibleIDs
            //  Que el print sea compatible con la impresora, es decir, que en la lista printsCompPrinter ya se detectó como compatible
            var printsCompFil = await DbContext.FilamentCompatibilities.AsNoTracking()
                .Include(fc => fc.PrintFKNavigation)
                .Where(fc => fc.PrintFK != null && filCompatibleIDs.Contains(fc.FilamentFK) && printsCompPrinterIDs.Contains((Guid)fc.PrintFK)).ToListAsync();
            var fil = filCompatible.Select(f => f.ID).ToList();

            //Rellenar el campo filaments del DTO
            response.Filaments = new List<Models.Slicer.Filament>();

            foreach (var filComp in filCompatible)
            {

                var compPrints = new List<Models.Slicer.Print>();

                foreach (var print in printsCompFil.Where(pcf => pcf.FilamentFK == filComp.ID))
                {
                    compPrints.Add(new Models.Slicer.Print()
                    {
                        Id = print.PrintFKNavigation.ID,
                        Name = print.PrintFKNavigation.Name,
                        iniKeyword = print.PrintFKNavigation.iniKeyword
                    });
                }

                response.Filaments.Add(new Models.Slicer.Filament()
                {
                    Id = filComp.ID,
                    Name = filComp.Name,
                    iniKeyword = filComp.iniKeyword,
                    MaterialFK = (Guid)filComp.MaterialFK, //No debería ser nulo tras pedirlo antes
                    CompatiblePrints = compPrints
                });
            }
            var otro = printsCompFil.Where(fc => fil.Contains(fc.FilamentFK)).ToList();
                




            return response;
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

        public async Task<List<Models.Material>> GetMaterials(string printerCompatible, Guid stockInNode = default(Guid))
        {
            var response = new List<Models.Material>();
            var printerId = (await DbContext.Printers.AsNoTracking().FirstOrDefaultAsync(p => p.Name == printerCompatible)).ID;
            var filamentsIds = await DbContext.Filaments.AsNoTracking()
                .Include(f => f.FilamentCompatibilities)
                .Include(f => f.MaterialFKNavigation)
                .Where(f => f.FilamentCompatibilities.Select(fc => fc.PrinterFK).Contains(printerId))
                .Select(f => f.MaterialFK)
                .ToListAsync();

            var mats = await DbContext.Materials.AsNoTracking()
                .Include(m => m.Stocks.Where(s => s.NodeFK == stockInNode))
                .Include(m => m.PropMats.Where(m => m.PropertyFKNavigation.Visible)).ThenInclude(m => m.PropertyFKNavigation)
                .Where(m => filamentsIds.Contains(m.ID))
                .ToListAsync();

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
        #endregion

        #region CHECKS
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
        #endregion

        #region UPDATE
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

            if (stock == null) return null;

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
        #endregion

        #region CREATE
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
        #endregion

        #region DELETE
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
        #endregion

        #region PROCESS
        public async Task ProcessSettingsBundle(List<string> lines, Dictionary<string, Guid> matching = null)
        {
            await this.DeleteAllSettings();

            string keyCompPrinters = "compatible_printers";
            string keyCompPrints = "compatible_prints";
            int found = 0;

            var prints = new List<Entities.Print>();
            var filaments = new List<Entities.Filament>();
            var printers = new List<Entities.Printer>();
            var filcompats = new List<Entities.FilamentCompatibility>();
            var printCompat = new List<Entities.PrintCompatibility>();

            //Bucle para localizar los presets en el bundle
            foreach (string line in lines)
            {
                //Si hay un paquete localizado, copia todas las líneas en el dict hasta la línea vacía
                if (found > 0)
                {
                    if (found == 1)
                    {
                        if (line == "")
                            found = 0;
                        else
                        {
                            var pair = DbHelpers.GetSetting(line);
                            prints.LastOrDefault().PrintSettings.Add(new Entities.PrintSetting()
                            {
                                ID = Guid.NewGuid(),
                                PrintFK = prints.LastOrDefault().ID,
                                iniKey = pair.Key,
                                iniValue = pair.Value
                            });
                        }
                    }
                    else if (found == 2)
                    {
                        if (line == "")
                            found = 0;
                        else
                        {
                            var pair = DbHelpers.GetSetting(line);
                            filaments.LastOrDefault().FilamentSettings.Add(new Entities.FilamentSetting()
                            {
                                ID = Guid.NewGuid(),
                                FilamentFK = filaments.LastOrDefault().ID,
                                iniKey = pair.Key,
                                iniValue = pair.Value
                            });
                        }
                    }
                    else if (found == 3)
                    {
                        if (line == "")
                            found = 0;
                        else
                        {
                            var pair = DbHelpers.GetSetting(line);
                            printers.LastOrDefault().PrinterSettings.Add(new Entities.PrinterSetting()
                            {
                                ID = Guid.NewGuid(),
                                PrinterFK = printers.LastOrDefault().ID,
                                iniKey = pair.Key,
                                iniValue = pair.Value
                            });
                        }
                    }
                }
                //Localizar etiquetas de presets
                if (line.StartsWith("[print:"))
                {
                    found = 1;
                    prints.Add(new Entities.Print()
                    {
                        ID = Guid.NewGuid(),
                        iniKeyword = line,
                        Name = DbHelpers.GetSettingBlockName(line)
                    });
                    prints.LastOrDefault().PrintSettings = new Collection<Entities.PrintSetting>();
                }
                if (line.StartsWith("[filament:"))
                {
                    found = 2;
                    filaments.Add(new Entities.Filament()
                    {
                        ID = Guid.NewGuid(),
                        iniKeyword = line,
                        Name = DbHelpers.GetSettingBlockName(line)
                    });
                    filaments.LastOrDefault().FilamentSettings = new Collection<Entities.FilamentSetting>();
                }
                if (line.StartsWith("[printer:"))
                {
                    found = 3;
                    printers.Add(new Entities.Printer()
                    {
                        ID = Guid.NewGuid(),
                        iniKeyword = line,
                        Name = DbHelpers.GetSettingBlockName(line)
                    });
                    printers.LastOrDefault().PrinterSettings = new Collection<Entities.PrinterSetting>();
                }
            }

            //Compatibilities
            foreach (var print in prints)
            {
                var listprinters = print.PrintSettings.FirstOrDefault(p => p.iniKey == keyCompPrinters)
                    .iniValue
                    .Split(";")
                    .ToList();
                foreach (string printerName in listprinters)
                {
                    var pPrinter = printers.FirstOrDefault(p => p.Name == printerName.Trim('\"'));
                    if (pPrinter != null)
                    {
                        printCompat.Add(new Entities.PrintCompatibility()
                        {
                            ID = Guid.NewGuid(),
                            PrintFK = print.ID,
                            PrinterFK = pPrinter.ID
                        });
                    }
                }
            }

            foreach (var filament in filaments)
            {
                var listprinters = filament.FilamentSettings.FirstOrDefault(p => p.iniKey == keyCompPrinters)
                    .iniValue
                    .Split(";")
                    .ToList();
                foreach (string printerName in listprinters)
                {
                    var pPrinter = printers.FirstOrDefault(p => p.Name == printerName.Trim('\"'));
                    if (pPrinter != null)
                    {
                        filcompats.Add(new Entities.FilamentCompatibility()
                        {
                            ID = Guid.NewGuid(),
                            FilamentFK = filament.ID,
                            PrinterFK = pPrinter.ID
                        });
                    }
                }
                var listprints = filament.FilamentSettings.FirstOrDefault(p => p.iniKey == keyCompPrints)
                    .iniValue
                    .Split(";")
                    .ToList();
                foreach (string printname in listprints)
                {
                    var pPrint = prints.FirstOrDefault(p => p.Name == printname.Trim('\"'));
                    if (pPrint != null)
                    {
                        filcompats.Add(new Entities.FilamentCompatibility()
                        {
                            ID = Guid.NewGuid(),
                            FilamentFK = filament.ID,
                            PrintFK = pPrint.ID
                        });
                    }
                }
            }

            if(matching != null)
            {
                foreach(var match in matching)
                {
                    var pointer = filaments.Find(f => f.Name == match.Key);
                    pointer.MaterialFK = match.Value;
                }
            }
            DbContext.AddRange(prints);
            DbContext.AddRange(filaments);
            DbContext.AddRange(printers);
            DbContext.AddRange(filcompats);
            DbContext.AddRange(printCompat);

            await DbContext.SaveChangesAsync();
        }
        public async Task ProcessSettingsBundle(MemoryStream ms, Dictionary<string, Guid> matching = null)
        {
            
            using (StreamReader reader = new StreamReader(ms))
            {
                ms.Seek(0, SeekOrigin.Begin);
                var fileString = reader.ReadToEnd();
                var lines = fileString.Split(Environment.NewLine).ToList();

                await this.ProcessSettingsBundle(lines, matching);
            }
        }


        private async Task DeleteAllSettings()
        {
            //Delete all
            DbContext.RemoveRange(DbContext.PrintCompatibilities);
            DbContext.RemoveRange(DbContext.FilamentCompatibilities);
            DbContext.RemoveRange(DbContext.PrinterSettings);
            DbContext.RemoveRange(DbContext.Printers);
            DbContext.RemoveRange(DbContext.FilamentSettings);
            DbContext.RemoveRange(DbContext.Filaments);
            DbContext.RemoveRange(DbContext.PrintSettings);
            DbContext.RemoveRange(DbContext.Prints);
            await DbContext.SaveChangesAsync();
        }

        #endregion
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

        internal static string GetSettingBlockName(string keyword)
        {
            var last = keyword.Split(":").Last();
            last = last.Remove(last.Length - 1);
            return last;
        }

        internal static KeyValuePair<string,string> GetSetting(string setting)
        {
            var parts = setting.Split("=");
            if (parts.Length == 2)
                return new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim());
            else
                return new KeyValuePair<string, string>(parts[0].Trim(), "");
        }
    }
}
