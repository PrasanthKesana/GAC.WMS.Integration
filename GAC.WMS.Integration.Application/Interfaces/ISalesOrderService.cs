using GAC.WMS.Integration.Application.Models.Requests;
using GAC.WMS.Integration.Domain.Models;

namespace GAC.WMS.Integration.Application.Interfaces
{
    public interface ISalesOrderService
    {
        Task<IEnumerable<SalesOrder>> GetAllSalesOrdersAsync();
        Task<SalesOrder> GetSalesOrderByIdAsync(int id);
        Task<SalesOrder> CreateSalesOrderAsync(SalesOrderDto salesOrderDto);
        Task UpdateSalesOrderAsync(SalesOrderDto salesOrderDto);
    }
}
