using AutoMapper;
using GAC.WMS.Integration.Application.Interfaces;
using GAC.WMS.Integration.Application.Models.Requests;
using GAC.WMS.Integration.Domain.Interfaces.Repositories;
using GAC.WMS.Integration.Domain.Models;

namespace GAC.WMS.Integration.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<Customer> CreateCustomerAsync(CustomerDto customerDto)
        {
            var customer = _mapper.Map<Customer>(customerDto);
            return await _customerRepository.AddAsync(customer);
        }

        public async Task UpdateCustomerAsync(CustomerDto customerDto)
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(customerDto.Id);
            if (existingCustomer == null)
                throw new ArgumentException("Customer not found");
            _mapper.Map(customerDto, existingCustomer);
            await _customerRepository.UpdateAsync(existingCustomer);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer != null)
            {
                await _customerRepository.DeleteAsync(customer);
            }
        }
    }
}
