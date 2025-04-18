using GAC.WMS.Integration.Domain.Interfaces.Repositories;
using GAC.WMS.Integration.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GAC.WMS.Integration.Infrastructure.Repository
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly IntegrationDbContext _context;

        public PurchaseOrderRepository(IntegrationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllAsync()
        {
            return await _context.PurchaseOrders
                .Include(po => po.Customer)
                .Include(po => po.Items)
                    .ThenInclude(item => item.Product)
                .ToListAsync();
        }

        public async Task<PurchaseOrder> GetByIdAsync(int id)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Customer)
                .Include(po => po.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(po => po.Id == id);
        }

        public async Task<PurchaseOrder> AddAsync(PurchaseOrder order)
        {
            try
            {
                _context.PurchaseOrders.Add(order);
                await _context.SaveChangesAsync();
                return order;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task UpdateAsync(PurchaseOrder order)
        {
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
