using AutoMapper;
using BackendProject2.Context;
using BackendProject2.Dto;
using BackendProject2.Models;
using Jose.native;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendProject2.Services.AuthServices
{
    public class AuthServices : IAuthServices
    {

        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public AuthServices(AppDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<bool> Register(UserRegistrationDto userRegistrationDto)
        {
            try
            {
                var IsExists = await _context.users.FirstOrDefaultAsync(a => a.UserEmail == userRegistrationDto.UserEmail);
                if (IsExists != null)
                {
                    return false;
                }


                var HashPassword = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password);


                var res = _mapper.Map<User>(userRegistrationDto);
                res.Password = HashPassword;
                _context.users.Add(res);
                await _context.SaveChangesAsync();

                return true;

            }
            catch (DbUpdateException dbex)
            {
                throw new Exception($"Database error: {dbex.InnerException?.Message ?? dbex.Message}");
            }
        }


        public async Task<UserResponseDto> Login(UserLoginDto userLogin_dto)
        {
            try
            {
                var u = await _context.users.FirstOrDefaultAsync(a => a.UserEmail == userLogin_dto.UserEmail);
                if (u == null)
                {
                    return new UserResponseDto { Error = "Not Found" };
                }

                var pass = validatePassword(userLogin_dto.Password, u.Password);
                if (!pass)
                {
                    return new UserResponseDto { Error = "Invalid Password" };
                }

                if (u.isBlocked == true)
                {
                    return new UserResponseDto { Error = "User Blocked" };
                }
                var Token = Generate_Token(u);
                return new UserResponseDto
                {
                    UserName = u.UserName,
                    Token = Token,
                    UserEmail = u.UserEmail,
                    Role = u.Role,
                    Id = u.Id
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        private string Generate_Token(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentails = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier ,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName ),
                new Claim(ClaimTypes.Role,user.Role),
                new Claim(ClaimTypes.Email,user.UserEmail)
            };

            var token = new JwtSecurityToken(
                claims: claim,
                signingCredentials: credentails,
                expires: DateTime.UtcNow.AddDays(1)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }



        private bool validatePassword(string password, string hashpassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashpassword);
        }




    }
}