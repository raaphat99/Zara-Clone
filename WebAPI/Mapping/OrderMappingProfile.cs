using AutoMapper;
using Domain.Models;
using WebAPI.DTOs;
namespace WebAPI.Mapping
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
          //  Order->OrderDTO
        CreateMap<Order, OrderDTO>()
            .ForMember(dest => dest.created, opt => opt.MapFrom(src => src.Created.ToString()))
            .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.customerName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.Name} {src.User.Surname}" : "Unknown"))
            .ForMember(dest => dest.items, opt => opt.MapFrom(src => src.OrderItems));

            // Order -> OrderDetailsDTO
            CreateMap<Order, OrderDetailsDTO>()
                .ForMember(dest => dest.orderDate, opt => opt.MapFrom(src => src.Created.Value))
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.paymentMethod, opt => opt.MapFrom(src => src.Payment != null ? src.Payment.PaymentMethod : "Payment method not available"))
                .ForMember(dest => dest.shippingCost, opt => opt.MapFrom(src => src.ShippingMethod != null ? src.ShippingMethod.ShippingCost : 0))
                .ForMember(dest => dest.customer, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.items, opt => opt.MapFrom(src => src.OrderItems));

            // OrderItem -> OrderItemDTO
            CreateMap<OrderItem, OrderItemDTO>()
          .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.ProductVariant != null && src.ProductVariant.Product != null ? src.ProductVariant.Product.Name : "Unknown Product"))
          .ForMember(dest => dest.productImage, opt => opt.MapFrom(src => src.ProductVariant != null && src.ProductVariant.ProductImage.FirstOrDefault() != null ? src.ProductVariant.ProductImage.FirstOrDefault().ImageUrl : "Image not available"))
          .ForMember(dest => dest.subtotal, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice))
          .ForMember(dest => dest.color, opt => opt.MapFrom(src => src.ProductVariant != null && src.ProductVariant.ProductColor != null ? src.ProductVariant.ProductColor.ToString() : "N/A"))
          .ForMember(dest => dest.size, opt => opt.MapFrom(src => src.ProductVariant != null && src.ProductVariant.Size != null ? src.ProductVariant.Size.ToString() : "N/A"));

            // User -> CustomerDTO
            CreateMap<User, CustomerDTO>()
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"))
                .ForMember(dest => dest.shippingAddress, opt => opt.MapFrom(src => src.Adresses.FirstOrDefault() != null
                    ? $"{src.Adresses.FirstOrDefault().Street}, {src.Adresses.FirstOrDefault().City}, {src.Adresses.FirstOrDefault().Country}"
                    : "Address not available"));
        }
    }
}
