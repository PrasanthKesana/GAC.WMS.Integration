using AutoMapper;
using GAC.WMS.Integration.Application.Interfaces;
using GAC.WMS.Integration.Application.Models.Requests;
using GAC.WMS.Integration.Domain.Interfaces.Repositories;
using GAC.WMS.Integration.Domain.Models;
using GAC.WMS.Integration.Infrastructure.Repository;

namespace GAC.WMS.Integration.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> GetProductByCodeAsync(string code)
        {
            return await _productRepository.GetByCodeAsync(code);
        }

        public async Task<Product> CreateProductAsync(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            return await _productRepository.AddAsync(product);
        }

        public async Task UpdateProductAsync(ProductDto productDto)
        {
            var existingProduct = await _productRepository.GetByCodeAsync(productDto.ProductCode);
            if (existingProduct == null)
                throw new ArgumentException("Product not found");
            _mapper.Map(productDto, existingProduct);
            
            await _productRepository.UpdateAsync(existingProduct);
        }

        public async Task DeleteProductAsync(string code)
        {
            var product = await _productRepository.GetByCodeAsync(code);
            if (product != null)
            {
                await _productRepository.DeleteAsync(product);
            }
        }
    }
}
