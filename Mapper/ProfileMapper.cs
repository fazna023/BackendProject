using AutoMapper;
using BackendProject2.Dto;
using BackendProject2.Models;

namespace BackendProject2.Mapper
{
    public class ProfileMapper:Profile
    {
        public ProfileMapper()
        {
            CreateMap<User, UserLoginDto>().ReverseMap();
            CreateMap<User, UserRegistrationDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CatAddDto>().ReverseMap();
            CreateMap<Product, ProductWithCategoryDto>()
                   .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src._Category.CategoryName))
            .ReverseMap();
            CreateMap<Product, AddProductDto>().ReverseMap();
            CreateMap<CartItems, CartViewDto>().ReverseMap();
            CreateMap<User, UserViewDto>().ReverseMap();
            CreateMap<WishList, WishListViewDto>().ReverseMap();
            CreateMap<UserAddress, AddNewAddressDto>().ReverseMap();
            CreateMap<UserAddress, GetAdressDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();
        }
    }
}
