
using BackendProject2.CloudinaryServices;
using BackendProject2.Context;
using BackendProject2.CustomMiddleweare;
using BackendProject2.Mapper;
using BackendProject2.Services.AddressServices;
using BackendProject2.Services.AuthServices;
using BackendProject2.Services.CartServices;
using BackendProject2.Services.CategoryServices;
using BackendProject2.Services.OrderServices;
using BackendProject2.Services.OrdersServices;
using BackendProject2.Services.ProductServices;
using BackendProject2.Services.AdminServices;
using BackendProject2.Services.WishListServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using BackendProject2.Dto;

namespace BackendProject2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAutoMapper(typeof(ProfileMapper));
            builder.Services.AddScoped<IAuthServices, AuthServices>();
            builder.Services.AddScoped<IProductServices, ProductServices>();
            builder.Services.AddScoped<ICategoryServices, CategoryServices>();
            builder.Services.AddScoped<ICloudinaryServices, CloudinaryService>();
            builder.Services.AddScoped<ICartService,CartService>();
            builder.Services.AddScoped<IAdminServices, AdminServices>();
            builder.Services.AddScoped<IWishListServices, WishListServices>();
            builder.Services.AddScoped<IOrderServices, OrderServices>();
            builder.Services.AddScoped<IAddressServices, AddressServices>();






            // Swagger Configuration
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and your JWT token."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });



            builder.Services.Configure<PaymentDto>(builder.Configuration.GetSection("Razorpay"));



            // JWT Authentication Configuration 
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
                o.RequireHttpsMetadata = false;  // Use true in production for security
                o.SaveToken = true;
            });

            builder.Services.AddDbContext<AppDbContext>(Options =>
            Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAutoMapper(typeof(ProfileMapper));


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy => policy.WithOrigins("http://localhost:5173") // Make sure this matches your frontend URL
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials()); // Allow credentials if you're using cookies or authorization headers
            });



            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");


            app.UseAuthorization();
            app.UseMiddleware<UserIdMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}