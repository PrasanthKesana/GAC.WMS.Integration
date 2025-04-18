using AutoMapper;
using GAC.WMS.Integration.Application.Interfaces;
using GAC.WMS.Integration.Application.Models.Requests;
using GAC.WMS.Integration.Application.Services;
using GAC.WMS.Integration.Domain.Interfaces.Repositories;
using GAC.WMS.Integration.Domain.Models;
using Moq;

namespace GAC.WMS.Integration.Application.Tests
{
    public class PurchaseOrderServiceTests
    {
        private readonly Mock<IPurchaseOrderRepository> _purchaseOrderRepoMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PurchaseOrderService _service;

        public PurchaseOrderServiceTests()
        {
            _purchaseOrderRepoMock = new Mock<IPurchaseOrderRepository>();
            _productRepoMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new PurchaseOrderService(_purchaseOrderRepoMock.Object, _productRepoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllPurchaseOrdersAsync_ShouldReturnAllOrders()
        {
            // Arrange
            var orders = new List<PurchaseOrder> { new PurchaseOrder { Id = 1 } };
            _purchaseOrderRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(orders);

            // Act
            var result = await _service.GetAllPurchaseOrdersAsync();

            // Assert
            Assert.Equal(orders, result);
            _purchaseOrderRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPurchaseOrderByIdAsync_ShouldReturnCorrectOrder()
        {
            var order = new PurchaseOrder { Id = 1 };
            _purchaseOrderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

            var result = await _service.GetPurchaseOrderByIdAsync(1);

            Assert.Equal(order, result);
            _purchaseOrderRepoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task CreatePurchaseOrderAsync_ShouldThrowException_WhenProductDoesNotExist()
        {
            var dto = new PurchaseOrderDto
            {
                CustomerId = 1,
                Items = new List<PurchaseOrderItemDto>
            {
                new PurchaseOrderItemDto { ProductCode = "INVALID", Quantity = 2 }
            }
            };

            _productRepoMock.Setup(p => p.GetByCodeAsync("INVALID")).ReturnsAsync((Product)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreatePurchaseOrderAsync(dto));
            Assert.Contains("Product with code INVALID not found", ex.Message);
        }

        [Fact]
        public async Task CreatePurchaseOrderAsync_ShouldCreateSuccessfully_WhenProductsValid()
        {
            var dto = new PurchaseOrderDto
            {
                OrderId = "PO001",
                CustomerId = 1,
                Items = new List<PurchaseOrderItemDto>
            {
                new PurchaseOrderItemDto { ProductCode = "P001", Quantity = 5 }
            }
            };

            var mappedOrder = new PurchaseOrder { Id = 1, OrderId = "PO001" };

            _productRepoMock.Setup(p => p.GetByCodeAsync("P001")).ReturnsAsync(new Product { Code = "P001" });
            _mapperMock.Setup(m => m.Map<PurchaseOrder>(dto)).Returns(mappedOrder);
            _purchaseOrderRepoMock.Setup(r => r.AddAsync(mappedOrder)).ReturnsAsync(mappedOrder);

            var result = await _service.CreatePurchaseOrderAsync(dto);

            Assert.Equal(mappedOrder, result);
            _productRepoMock.Verify(p => p.GetByCodeAsync("P001"), Times.Once);
            _purchaseOrderRepoMock.Verify(r => r.AddAsync(mappedOrder), Times.Once);
        }

        [Fact]
        public async Task UpdatePurchaseOrderAsync_ExistingPurchaseOrder_ShouldCallRepositoryUpdateAndMap()
        {
            // Arrange
            var purchaseOrderDto = new PurchaseOrderDto { Id = 1, OrderId = "PO001", ProcessingDate = DateTime.Now };
            var existingPurchaseOrder = new PurchaseOrder { Id = 1, OrderId = "PO001", ProcessingDate = DateTime.Now.AddDays(-1) };

            _purchaseOrderRepoMock.Setup(repo => repo.GetByIdAsync(purchaseOrderDto.Id)).ReturnsAsync(existingPurchaseOrder);
            _purchaseOrderRepoMock.Setup(repo => repo.UpdateAsync(existingPurchaseOrder)).Returns(Task.CompletedTask);
            _mapperMock.Setup(mapper => mapper.Map(purchaseOrderDto, existingPurchaseOrder));

            // Act
            await _service.UpdatePurchaseOrderAsync(purchaseOrderDto);

            // Assert
            _purchaseOrderRepoMock.Verify(repo => repo.GetByIdAsync(purchaseOrderDto.Id), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map(purchaseOrderDto, existingPurchaseOrder), Times.Once);
            _purchaseOrderRepoMock.Verify(repo => repo.UpdateAsync(existingPurchaseOrder), Times.Once);
        }

        [Fact]
        public async Task UpdatePurchaseOrderAsync_NonExistingPurchaseOrder_ShouldThrowArgumentException()
        {
            // Arrange
            var purchaseOrderDto = new PurchaseOrderDto { Id = 2, OrderId = "PO002" };
            _purchaseOrderRepoMock.Setup(repo => repo.GetByIdAsync(purchaseOrderDto.Id)).ReturnsAsync((PurchaseOrder)null!);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _service.UpdatePurchaseOrderAsync(purchaseOrderDto));

            // Assert (Verification that other methods were NOT called)
            _mapperMock.Verify(mapper => mapper.Map(It.IsAny<PurchaseOrderDto>(), It.IsAny<PurchaseOrder>()), Times.Never);
            _purchaseOrderRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<PurchaseOrder>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePurchaseOrderAsync_RepositoryThrowsException_ShouldBubbleUpException()
        {
            // Arrange
            var purchaseOrderDto = new PurchaseOrderDto { Id = 3, OrderId = "PO003" };
            var existingPurchaseOrder = new PurchaseOrder { Id = 3, OrderId = "PO003" };
            var expectedException = new Exception("Database error");

            _purchaseOrderRepoMock.Setup(repo => repo.GetByIdAsync(purchaseOrderDto.Id)).ReturnsAsync(existingPurchaseOrder);
            _mapperMock.Setup(mapper => mapper.Map(purchaseOrderDto, existingPurchaseOrder));
            _purchaseOrderRepoMock.Setup(repo => repo.UpdateAsync(existingPurchaseOrder)).ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _service.UpdatePurchaseOrderAsync(purchaseOrderDto));
            Assert.Equal(expectedException.Message, exception.Message);

            // Assert (Verification of calls)
            _purchaseOrderRepoMock.Verify(repo => repo.GetByIdAsync(purchaseOrderDto.Id), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map(purchaseOrderDto, existingPurchaseOrder), Times.Once);
            _purchaseOrderRepoMock.Verify(repo => repo.UpdateAsync(existingPurchaseOrder), Times.Once);
        }

        [Fact]
        public async Task ProcessPurchaseOrderAsync_ShouldUpdateModifiedDate()
        {
            var order = new PurchaseOrder { Id = 1 };
            _purchaseOrderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

            await _service.ProcessPurchaseOrderAsync(1);

            _purchaseOrderRepoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
            _purchaseOrderRepoMock.Verify(r => r.UpdateAsync(It.Is<PurchaseOrder>(o => o.ModifiedDate.HasValue)), Times.Once);
        }

        [Fact]
        public async Task ProcessPurchaseOrderAsync_ShouldThrow_WhenOrderNotFound()
        {
            _purchaseOrderRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((PurchaseOrder)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.ProcessPurchaseOrderAsync(999));

            Assert.Equal("Purchase order not found", ex.Message);
        }

    }
}
