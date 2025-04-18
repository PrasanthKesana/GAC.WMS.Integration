using AutoMapper;
using GAC.WMS.Integration.Application.Interfaces;
using GAC.WMS.Integration.Application.Models.Requests;
using GAC.WMS.Integration.Domain.Interfaces.Repositories;
using GAC.WMS.Integration.Domain.Models;

namespace GAC.WMS.Integration.Application.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public PurchaseOrderService(
            IPurchaseOrderRepository purchaseOrderRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrdersAsync()
        {
            return await _purchaseOrderRepository.GetAllAsync();
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id)
        {
            return await _purchaseOrderRepository.GetByIdAsync(id);
        }

        public async Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrderDto purchaseOrderDto)
        {
            // Validate all products exist
            foreach (var item in purchaseOrderDto.Items)
            {
                var product = await _productRepository.GetByCodeAsync(item.ProductCode);
                if (product == null)
                {
                    throw new ArgumentException($"Product with code {item.ProductCode} not found");
                }
            }
            var purchaseOrder = _mapper.Map<PurchaseOrder>(purchaseOrderDto);

            return await _purchaseOrderRepository.AddAsync(purchaseOrder);
        }

        public async Task UpdatePurchaseOrderAsync(PurchaseOrderDto purchaseOrderDto)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(purchaseOrderDto.Id);
            if (purchaseOrder == null)
                throw new ArgumentException("PurchaseOrder not found");
            _mapper.Map(purchaseOrderDto, purchaseOrder);
            await _purchaseOrderRepository.UpdateAsync(purchaseOrder);
        }

        public async Task ProcessPurchaseOrderAsync(int id)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new ArgumentException("Purchase order not found");
            }

            order.ModifiedDate = DateTime.UtcNow;
            await _purchaseOrderRepository.UpdateAsync(order);
        }
    }
}
