using AutoMapper;
using GAC.WMS.Integration.Application.Models.Requests;
using GAC.WMS.Integration.Application.Services;
using GAC.WMS.Integration.Domain.Interfaces.Repositories;
using GAC.WMS.Integration.Domain.Models;
using Moq;

namespace GAC.WMS.Integration.Application.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly ProductService _productService;
        private readonly Mock<IMapper> _mapperMock;

        public ProductServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _productRepoMock = new Mock<IProductRepository>();
            _productService = new ProductService(_productRepoMock.Object, _mapperMock.Object );
        }
        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product> { new Product { Code = "P001", Title = "Widget" } };
            _productRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.Equal(products, result);
            _productRepoMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProductByCodeAsync_ShouldReturnProduct()
        {
            // Arrange
            var product = new Product { Code = "P001", Title = "Widget" };
            _productRepoMock.Setup(repo => repo.GetByCodeAsync("P001")).ReturnsAsync(product);

            // Act
            var result = await _productService.GetProductByCodeAsync("P001");

            // Assert
            Assert.Equal(product, result);
            _productRepoMock.Verify(repo => repo.GetByCodeAsync("P001"), Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldMapAndAddProduct()
        {
            // Arrange
            var dto = new ProductDto { ProductCode = "P001", Title = "Widget" };
            var product = new Product { Code = "P001", Title = "Widget" };

            _mapperMock.Setup(m => m.Map<Product>(dto)).Returns(product);
            _productRepoMock.Setup(repo => repo.AddAsync(product)).ReturnsAsync(product);

            // Act
            var result = await _productService.CreateProductAsync(dto);

            // Assert
            Assert.Equal(product, result);
            _mapperMock.Verify(m => m.Map<Product>(dto), Times.Once);
            _productRepoMock.Verify(repo => repo.AddAsync(product), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ExistingProduct_ShouldCallRepositoryUpdateAndMap()
        {
            // Arrange
            var productDto = new ProductDto { ProductCode = "PROD001", Title = "Updated Title", Description = "Updated Description" };
            var existingProduct = new Product { Code = "PROD001", Title = "Original Title", Description = "Original Description" };

            _productRepoMock.Setup(repo => repo.GetByCodeAsync(productDto.ProductCode)).ReturnsAsync(existingProduct);
            _productRepoMock.Setup(repo => repo.UpdateAsync(existingProduct)).Returns(Task.CompletedTask);
            _mapperMock.Setup(mapper => mapper.Map(productDto, existingProduct));

            // Act
            await _productService.UpdateProductAsync(productDto);

            // Assert
            _productRepoMock.Verify(repo => repo.GetByCodeAsync(productDto.ProductCode), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map(productDto, existingProduct), Times.Once);
            _productRepoMock.Verify(repo => repo.UpdateAsync(existingProduct), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_NonExistingProduct_ShouldThrowArgumentException()
        {
            // Arrange
            var productDto = new ProductDto { ProductCode = "PROD002", Title = "New Title" };
            _productRepoMock.Setup(repo => repo.GetByCodeAsync(productDto.ProductCode)).ReturnsAsync((Product)null!);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _productService.UpdateProductAsync(productDto));

            // Assert 
            _mapperMock.Verify(mapper => mapper.Map(It.IsAny<ProductDto>(), It.IsAny<Product>()), Times.Never);
            _productRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProductAsync_RepositoryThrowsException_ShouldBubbleUpException()
        {
            // Arrange
            var productDto = new ProductDto { ProductCode = "PROD003", Title = "Problematic Title" };
            var existingProduct = new Product { Code = "PROD003", Title = "Original" };
            var expectedException = new Exception("Database error");

            _productRepoMock.Setup(repo => repo.GetByCodeAsync(productDto.ProductCode)).ReturnsAsync(existingProduct);
            _mapperMock.Setup(mapper => mapper.Map(productDto, existingProduct));
            _productRepoMock.Setup(repo => repo.UpdateAsync(existingProduct)).ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _productService.UpdateProductAsync(productDto));
            Assert.Equal(expectedException.Message, exception.Message);

            // Assert
            _productRepoMock.Verify(repo => repo.GetByCodeAsync(productDto.ProductCode), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map(productDto, existingProduct), Times.Once);
            _productRepoMock.Verify(repo => repo.UpdateAsync(existingProduct), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldDeleteIfExists()
        {
            // Arrange
            var product = new Product { Code = "P001" };
            _productRepoMock.Setup(repo => repo.GetByCodeAsync("P001")).ReturnsAsync(product);

            // Act
            await _productService.DeleteProductAsync("P001");

            // Assert
            _productRepoMock.Verify(repo => repo.GetByCodeAsync("P001"), Times.Once);
            _productRepoMock.Verify(repo => repo.DeleteAsync(product), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldNotDeleteIfNotFound()
        {
            // Arrange
            _productRepoMock.Setup(repo => repo.GetByCodeAsync("P404")).ReturnsAsync((Product)null);

            // Act
            await _productService.DeleteProductAsync("P404");

            // Assert
            _productRepoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Product>()), Times.Never);
        }
    }
}
