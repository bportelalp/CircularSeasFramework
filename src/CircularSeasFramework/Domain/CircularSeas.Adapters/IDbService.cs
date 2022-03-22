using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircularSeas.Adapters
{
    public interface IDbService
    {

        Task<Models.DTO.PrintDTO> GetPrintDTO(string printerCompatible, Guid stockInNode);
        Task<List<Models.Material>> GetMaterials(bool includeProperties = true, Guid stockInNode = new Guid(), bool forUsers = true);
        Task<List<Models.Material>> GetMaterials(string printerCompatible, Guid stockInNode = default(Guid));
        Task<Models.Material> GetMaterialDetail(Guid id);
        Task<List<Models.Property>> GetProperties();
        Task<Models.Property> GetPropertyDetail(Guid id);
        Task<List<Models.Node>> GetNodes();
        Task<Models.Material> GetMaterialSchema();
        Task<List<Models.Order>> GetOrders(int status = 0, Guid specificNode = default(Guid), bool includeMaterial = true);
        Task<Models.Order> GetOrder(Guid orderId);
        Task<SortedDictionary<string, string>> GetSlicerConfig(string printer, string filament, string print);
        Task<List<Models.Material>> CheckBadMaterialsVisible(Guid propertyId);
        Task UpdateMaterial(Models.Material material);
        Task UpdateProperty(Models.Property property);
        Task<Models.Order> UpdateOrder(Models.Order order);
        Task<Models.Stock> UpdateStock(Models.Order order);
        Task<Models.Stock> UpdateStock(Guid nodeId, Guid materialId, int amount);
        Task UpdateMaterialEvaluations(Models.Material material);
        Task UpdateVisibility(Guid id, bool visible);
        Task<Models.Property> CreateProperty(Models.Property property);
        Task<Models.Material> CreateMaterial(Models.Material material);
        Task<Models.Order> CreateOrder(Models.Order order);
        Task DeleteProperty(Guid id);
        Task DeleteMaterial(Guid id);
        Task ProcessSettingsBundle(List<string> lines, Dictionary<string, Guid> matching = null);
        Task ProcessSettingsBundle(MemoryStream ms, Dictionary<string, Guid> matching = null);

    }
}
