using GAC.WMS.Integration.Application.Models.Requests;
using GAC.WMS.Integration.Domain.Models;

namespace GAC.WMS.Integration.Application.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrdersAsync();
        Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id);
        Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrderDto purchaseOrderDto);
        Task UpdatePurchaseOrderAsync(PurchaseOrderDto purchaseOrderDto);
        Task ProcessPurchaseOrderAsync(int id);
    }
}
