using BackendProject2.Dto;

namespace BackendProject2.Services.AuthServices
{
    public interface IAuthServices
    {
        Task<bool> Register(UserRegistrationDto userRegistrationDto);
        Task<UserResponseDto> Login(UserLoginDto userLoginDto);
    }
}
