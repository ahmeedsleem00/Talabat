using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using UserAddress = Talabat.Core.Entities.Identity.Address;
using OrderAddress = Talabat.Core.Entities.Order.Address;
using Talabat.Core.Entities.Order;

namespace Talabat.APIs.Helpers
{
	public class MappingProfile : Profile
	{
        public MappingProfile()
        {

            CreateMap<Product, ProductToReturnDto>()
                .ForMember( d  =>  d.productBrand , o => o.MapFrom( s => s.ProductBrand.Name))
				.ForMember(d => d.ProductType, o => o.MapFrom(s => s.ProductType.Name)) 
                .ForMember(d => d.PictureUrl,o => o.MapFrom<ProductPictureUrlResolver>()) ;

            CreateMap<UserAddress, AddressDto>().ReverseMap();
            CreateMap<OrderAddress, AddressDto>().ReverseMap()
                .ForMember(d => d.FirstName, O => O.MapFrom(s => s.FName))
                .ForMember(d => d.LastName, O => O.MapFrom(s => s.LName));


            CreateMap<Order, OrderToRetunDto>()
                 .ForMember(d => d.DeliveryMethod, O => O.MapFrom(S => S.DeliveryMethod.ShortName))
                 .ForMember(d => d.DeliveryMethod, O => O.MapFrom(S => S.DeliveryMethod.Cost))
                 ;

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, O => O.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.ProductName, O => O.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.PictureUrl, O => O.MapFrom(s => s.Product.PictureUrl))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>())
                ;

            CreateMap<CustomerBasket, CustomerBasketDto>().ReverseMap();
        }



    }
}
