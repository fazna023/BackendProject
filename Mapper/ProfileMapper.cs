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

        }
    }
}
