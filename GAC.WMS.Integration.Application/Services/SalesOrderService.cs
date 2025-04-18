using AutoMapper;
using GAC.WMS.Integration.Application.Interfaces;
using GAC.WMS.Integration.Application.Models.Requests;
using GAC.WMS.Integration.Domain.Interfaces.Repositories;
using GAC.WMS.Integration.Domain.Models;
using GAC.WMS.Integration.Infrastructure.Repository;

namespace GAC.WMS.Integration.Application.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public SalesOrderService(
            ISalesOrderRepository salesOrderRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _salesOrderRepository = salesOrderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SalesOrder>> GetAllSalesOrdersAsync()
        {
            return await _salesOrderRepository.GetAllAsync();
        }

        public async Task<SalesOrder> GetSalesOrderByIdAsync(int id)
        {
            return await _salesOrderRepository.GetByIdAsync(id);
        }

        public async Task<SalesOrder> CreateSalesOrderAsync(SalesOrderDto salesOrderDto)
        {
            // Validate all products exist
            foreach (var item in salesOrderDto.Items)
            {
                var product = await _productRepository.GetByCodeAsync(item.ProductCode);
                if (product == null)
                {
                    throw new ArgumentException($"Product with code {item.ProductCode} not found");
                }
            }
            var salesOrder = _mapper.Map<SalesOrder>(salesOrderDto);

            return await _salesOrderRepository.AddAsync(salesOrder);
        }

        public async Task UpdateSalesOrderAsync(SalesOrderDto  salesOrderDto)
        {
            var salesOrder = await _salesOrderRepository.GetByIdAsync(salesOrderDto.Id);
            if (salesOrder == null)
                throw new ArgumentException("SalesOrder not found");
            _mapper.Map(salesOrderDto, salesOrder);
            await _salesOrderRepository.UpdateAsync(salesOrder);
        }

    }
}
