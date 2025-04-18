using GAC.WMS.Integration.Domain.Models;

namespace GAC.WMS.Integration.Domain.Interfaces
{
    public interface IApiClient
    {
        Task<bool> SendCustomerAsync(Customer customer);
        Task<bool> SendProductAsync(Product product);
    }
}
