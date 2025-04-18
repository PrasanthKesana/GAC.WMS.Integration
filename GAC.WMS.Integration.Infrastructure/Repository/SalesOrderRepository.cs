using GAC.WMS.Integration.Domain.Interfaces.Repositories;
using GAC.WMS.Integration.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GAC.WMS.Integration.Infrastructure.Repository
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly IntegrationDbContext _context;

        public SalesOrderRepository(IntegrationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesOrder>> GetAllAsync()
        {
            return await _context.SalesOrders
                .Include(so => so.Customer)
                .Include(so => so.Items)
                    .ThenInclude(item => item.Product)
                .ToListAsync();
        }

        public async Task<SalesOrder> GetByIdAsync(int id)
        {
            return await _context.SalesOrders
                .Include(so => so.Customer)
                .Include(so => so.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(so => so.Id == id);
        }

        public async Task<SalesOrder> AddAsync(SalesOrder order)
        {
            _context.SalesOrders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateAsync(SalesOrder order)
        {
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
