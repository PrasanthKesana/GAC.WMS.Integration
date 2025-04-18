using GAC.WMS.Integration.Application.Models.Requests;
using GAC.WMS.Integration.Domain.Models;

namespace GAC.WMS.Integration.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByCodeAsync(string code);
        Task<Product> CreateProductAsync(ProductDto productDto);
        Task UpdateProductAsync(ProductDto product);
        Task DeleteProductAsync(string code);
    }
}
