using AutoMapper;
using GAC.WMS.Integration.Application.Models.Requests;
using GAC.WMS.Integration.Application.Services;
using GAC.WMS.Integration.Domain.Interfaces.Repositories;
using GAC.WMS.Integration.Domain.Models;
using Moq;

namespace GAC.WMS.Integration.Application.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly CustomerService _customerService;
        private readonly Mock<IMapper> _mapperMock;

        public CustomerServiceTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _mapperMock = new Mock<IMapper>();
            _customerService = new CustomerService(_customerRepositoryMock.Object, _mapperMock.Object);
        }
        [Fact]
        public async Task GetAllCustomersAsync_ShouldReturnAllCustomers()
        {
            // Arrange
            var customers = new List<Customer> { new Customer { Id = 1, Name = "John", CustomerIdentifier = "CUST1" } };
            _customerRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(customers);

            // Act
            var result = await _customerService.GetAllCustomersAsync();

            // Assert
            Assert.Equal(customers, result);
            _customerRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ShouldReturnCorrectCustomer()
        {
            // Arrange
            var customer = new Customer { Id = 1, Name = "John", CustomerIdentifier = "CUST1" };
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(customer);

            // Act
            var result = await _customerService.GetCustomerByIdAsync(1);

            // Assert
            Assert.Equal(customer, result);
            _customerRepositoryMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task CreateCustomerAsync_ShouldMapAndCreateCustomer()
        {
            // Arrange
            var dto = new CustomerDto { CustomerIdentifier = "CUST2", Name = "Alice", Address = "123 Street" };
            var customer = new Customer { Id = 2, Name = "Alice", CustomerIdentifier = "CUST2" };

            _mapperMock.Setup(m => m.Map<Customer>(dto)).Returns(customer);
            _customerRepositoryMock.Setup(repo => repo.AddAsync(customer)).ReturnsAsync(customer);

            // Act
            var result = await _customerService.CreateCustomerAsync(dto);

            // Assert
            Assert.Equal(customer, result);
            _mapperMock.Verify(m => m.Map<Customer>(dto), Times.Once);
            _customerRepositoryMock.Verify(repo => repo.AddAsync(customer), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerAsync_ExistingCustomer_ShouldCallRepositoryUpdateAndMap()
        {
            // Arrange
            var customerDto = new CustomerDto { Id = 1, Name = "Updated Name", Address = "Updated Address" };
            var existingCustomer = new Customer { Id = 1, Name = "Original Name", Address = "Original Address" };

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerDto.Id)).ReturnsAsync(existingCustomer);
            _customerRepositoryMock.Setup(repo => repo.UpdateAsync(existingCustomer)).Returns(Task.CompletedTask);
            _mapperMock.Setup(mapper => mapper.Map(customerDto, existingCustomer));

            // Act
            await _customerService.UpdateCustomerAsync(customerDto);

            // Assert
            _customerRepositoryMock.Verify(repo => repo.GetByIdAsync(customerDto.Id), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map(customerDto, existingCustomer), Times.Once);
            _customerRepositoryMock.Verify(repo => repo.UpdateAsync(existingCustomer), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerAsync_NonExistingCustomer_ShouldThrowArgumentException()
        {
            // Arrange
            var customerDto = new CustomerDto { Id = 2, Name = "New Customer" };
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerDto.Id)).ReturnsAsync((Customer)null!);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _customerService.UpdateCustomerAsync(customerDto));

            // Assert (Verification that other methods were NOT called)
            _mapperMock.Verify(mapper => mapper.Map(It.IsAny<CustomerDto>(), It.IsAny<Customer>()), Times.Never);
            _customerRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCustomerAsync_RepositoryThrowsException_ShouldBubbleUpException()
        {
            // Arrange
            var customerDto = new CustomerDto { Id = 3, Name = "Problematic Customer" };
            var existingCustomer = new Customer { Id = 3, Name = "Original" };
            var expectedException = new Exception("Database error");

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerDto.Id)).ReturnsAsync(existingCustomer);
            _mapperMock.Setup(mapper => mapper.Map(customerDto, existingCustomer));
            _customerRepositoryMock.Setup(repo => repo.UpdateAsync(existingCustomer)).ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _customerService.UpdateCustomerAsync(customerDto));
            Assert.Equal(expectedException.Message, exception.Message);

            // Assert (Verification of calls)
            _customerRepositoryMock.Verify(repo => repo.GetByIdAsync(customerDto.Id), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map(customerDto, existingCustomer), Times.Once);
            _customerRepositoryMock.Verify(repo => repo.UpdateAsync(existingCustomer), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomerAsync_ShouldCallDelete_WhenCustomerExists()
        {
            // Arrange
            var customer = new Customer { Id = 1, Name = "ToDelete" };
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

            // Act
            await _customerService.DeleteCustomerAsync(1);

            // Assert
            _customerRepositoryMock.Verify(r => r.DeleteAsync(customer), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomerAsync_ShouldNotCallDelete_WhenCustomerDoesNotExist()
        {
            // Arrange
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Customer)null);

            // Act
            await _customerService.DeleteCustomerAsync(999);

            // Assert
            _customerRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Customer>()), Times.Never);
        }

    }
}
