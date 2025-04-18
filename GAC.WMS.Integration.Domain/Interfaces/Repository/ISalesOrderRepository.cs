using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAC.WMS.Integration.Domain.Models;

namespace GAC.WMS.Integration.Domain.Interfaces.Repositories
{
    public interface ISalesOrderRepository
    {
        Task<IEnumerable<SalesOrder>> GetAllAsync();
        Task<SalesOrder> GetByIdAsync(int id);
        Task<SalesOrder> AddAsync(SalesOrder order);
        Task UpdateAsync(SalesOrder order);

    }
}
