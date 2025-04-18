using AutoMapper;
using GAC.WMS.Integration.Application.Models.Requests;
using GAC.WMS.Integration.Domain.Models;

namespace GAC.WMS.Integration.Application.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Customer
            CreateMap<CustomerDto, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Customer, CustomerDto>();

            // Product
            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.ProductCode));

            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.Code));

            // Purchase Order
            CreateMap<PurchaseOrderDto, PurchaseOrder>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<PurchaseOrderItemDto, PurchaseOrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrder, opt => opt.Ignore());

            // Sales Order
            CreateMap<SalesOrderDto, SalesOrder>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<SalesOrderItemDto, SalesOrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.SalesOrder, opt => opt.Ignore());

            // Reverse maps
            CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>();
            CreateMap<SalesOrderItem, SalesOrderItemDto>()
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProductCode));
        }
    }
}
