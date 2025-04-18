using AutoMapper;
using GAC.WMS.Integration.Application.Interfaces;
using GAC.WMS.Integration.Application.Models.Requests;
using GAC.WMS.Integration.Application.Services;
using GAC.WMS.Integration.Domain.Interfaces.Repositories;
using GAC.WMS.Integration.Domain.Models;
using Moq;

namespace GAC.WMS.Integration.Application.Tests
{
    public class SalesOrderServiceTests
    {
        private readonly Mock<ISalesOrderRepository> _salesOrderRepoMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly SalesOrderService _service;

        public SalesOrderServiceTests()
        {
            _salesOrderRepoMock = new Mock<ISalesOrderRepository>();
            _productRepoMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new SalesOrderService(_salesOrderRepoMock.Object, _productRepoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllSalesOrdersAsync_ShouldReturnAllOrders()
        {
            var orders = new List<SalesOrder> { new SalesOrder { Id = 1 } };
            _salesOrderRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(orders);

            var result = await _service.GetAllSalesOrdersAsync();

            Assert.Equal(orders, result);
            _salesOrderRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetSalesOrderByIdAsync_ShouldReturnCorrectOrder()
        {
            var order = new SalesOrder { Id = 1 };
            _salesOrderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

            var result = await _service.GetSalesOrderByIdAsync(1);

            Assert.Equal(order, result);
            _salesOrderRepoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task CreateSalesOrderAsync_ShouldThrow_WhenProductDoesNotExist()
        {
            var dto = new SalesOrderDto
            {
                CustomerId = 1,
                ShipmentAddress = "Test Street",
                Items = new List<SalesOrderItemDto>
            {
                new SalesOrderItemDto { Quantity = 5, ProductCode = "INVALID" }
            }
            };

            _productRepoMock.Setup(p => p.GetByCodeAsync("INVALID")).ReturnsAsync((Product)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateSalesOrderAsync(dto));
            Assert.Contains("Product with code INVALID not found", ex.Message);
        }

        [Fact]
        public async Task CreateSalesOrderAsync_ShouldMapAndSave_WhenProductsAreValid()
        {
            var dto = new SalesOrderDto
            {
                OrderId = "SO123",
                CustomerId = 1,
                ShipmentAddress = "123 Road",
                Items = new List<SalesOrderItemDto>
            {
                new SalesOrderItemDto { ProductCode = "P001", Quantity = 3 }
            }
            };

            var mappedOrder = new SalesOrder { Id = 1, OrderId = "SO123" };

            _productRepoMock.Setup(p => p.GetByCodeAsync("P001")).ReturnsAsync(new Product { Code = "P001" });
            _mapperMock.Setup(m => m.Map<SalesOrder>(dto)).Returns(mappedOrder);
            _salesOrderRepoMock.Setup(r => r.AddAsync(mappedOrder)).ReturnsAsync(mappedOrder);

            var result = await _service.CreateSalesOrderAsync(dto);

            Assert.Equal(mappedOrder, result);
            _salesOrderRepoMock.Verify(r => r.AddAsync(mappedOrder), Times.Once);
        }

        [Fact]
        public async Task UpdateSalesOrderAsync_ExistingSalesOrder_ShouldCallRepositoryUpdateAndMap()
        {
            // Arrange
            var salesOrderDto = new SalesOrderDto { Id = 1, OrderId = "SO001", ShipmentAddress = "Updated Address" };
            var existingSalesOrder = new SalesOrder { Id = 1, OrderId = "SO001", ShipmentAddress = "Original Address" };

            _salesOrderRepoMock.Setup(repo => repo.GetByIdAsync(salesOrderDto.Id)).ReturnsAsync(existingSalesOrder);
            _salesOrderRepoMock.Setup(repo => repo.UpdateAsync(existingSalesOrder)).Returns(Task.CompletedTask);
            _mapperMock.Setup(mapper => mapper.Map(salesOrderDto, existingSalesOrder));

            // Act
            await _service.UpdateSalesOrderAsync(salesOrderDto);

            // Assert
            _salesOrderRepoMock.Verify(repo => repo.GetByIdAsync(salesOrderDto.Id), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map(salesOrderDto, existingSalesOrder), Times.Once);
            _salesOrderRepoMock.Verify(repo => repo.UpdateAsync(existingSalesOrder), Times.Once);
        }

        [Fact]
        public async Task UpdateSalesOrderAsync_NonExistingSalesOrder_ShouldThrowArgumentException()
        {
            // Arrange
            var salesOrderDto = new SalesOrderDto { Id = 2, OrderId = "SO002" };
            _salesOrderRepoMock.Setup(repo => repo.GetByIdAsync(salesOrderDto.Id)).ReturnsAsync((SalesOrder)null!);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _service.UpdateSalesOrderAsync(salesOrderDto));

            // Assert (Verification that other methods were NOT called)
            _mapperMock.Verify(mapper => mapper.Map(It.IsAny<SalesOrderDto>(), It.IsAny<SalesOrder>()), Times.Never);
            _salesOrderRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<SalesOrder>()), Times.Never);
        }

        [Fact]
        public async Task UpdateSalesOrderAsync_RepositoryThrowsException_ShouldBubbleUpException()
        {
            // Arrange
            var salesOrderDto = new SalesOrderDto { Id = 3, OrderId = "SO003" };
            var existingSalesOrder = new SalesOrder { Id = 3, OrderId = "SO003" };
            var expectedException = new Exception("Database error");

            _salesOrderRepoMock.Setup(repo => repo.GetByIdAsync(salesOrderDto.Id)).ReturnsAsync(existingSalesOrder);
            _mapperMock.Setup(mapper => mapper.Map(salesOrderDto, existingSalesOrder));
            _salesOrderRepoMock.Setup(repo => repo.UpdateAsync(existingSalesOrder)).ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _service.UpdateSalesOrderAsync(salesOrderDto));
            Assert.Equal(expectedException.Message, exception.Message);

            // Assert (Verification of calls)
            _salesOrderRepoMock.Verify(repo => repo.GetByIdAsync(salesOrderDto.Id), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map(salesOrderDto, existingSalesOrder), Times.Once);
            _salesOrderRepoMock.Verify(repo => repo.UpdateAsync(existingSalesOrder), Times.Once);
        }

    }
}
