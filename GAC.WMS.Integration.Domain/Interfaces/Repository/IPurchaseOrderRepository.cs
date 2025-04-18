using GAC.WMS.Integration.Domain.Models;

namespace GAC.WMS.Integration.Domain.Interfaces.Repositories
{
    public interface IPurchaseOrderRepository
    {
        Task<IEnumerable<PurchaseOrder>> GetAllAsync();
        Task<PurchaseOrder> GetByIdAsync(int id);
        Task<PurchaseOrder> AddAsync(PurchaseOrder order);
        Task UpdateAsync(PurchaseOrder order);
    }
}
