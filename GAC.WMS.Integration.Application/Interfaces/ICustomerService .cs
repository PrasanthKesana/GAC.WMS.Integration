using GAC.WMS.Integration.Application.Models.Requests;
using GAC.WMS.Integration.Domain.Models;

namespace GAC.WMS.Integration.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<Customer> CreateCustomerAsync(CustomerDto customer);
        Task UpdateCustomerAsync(CustomerDto customerDto);
        Task DeleteCustomerAsync(int id);
    }
}
