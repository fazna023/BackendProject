using AutoMapper;
using BackendProject2.ApiResponse;
using BackendProject2.CloudinaryServices;
using BackendProject2.Context;
using BackendProject2.Dto;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;

using Product = BackendProject2.Models.Product;

namespace BackendProject2.Services.AdminServices
{
    public class AdminServices : IAdminServices
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminServices> _logger;
        private readonly IMapper _mapper;
        private readonly ICloudinaryServices _cloudinary;

        public AdminServices(AppDbContext context, ILogger<AdminServices> logger, IMapper mapper,ICloudinaryServices cloudinary)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }


        public async Task<ApiResponse<bool>> BlockOrUnblock(int id)
        {
            var user = await _context.users.FindAsync(id);
            if (user == null)
            {
                return new ApiResponse<bool>(
                    isSuccess: false,
                    message: "user not found",
                    data: false,
                    error: "no user exist with the provided Id."

                    );
            }
            user.isBlocked = !user.isBlocked;
            await _context.SaveChangesAsync();

            string Status = user.isBlocked ? "user Blocked" : "user Unblocked";

            return new ApiResponse<bool>(
                isSuccess: true,
                message: Status,
                data: user.isBlocked,
                error: null
                );

        }

        public async Task<List<UserViewDto>> GetUser()
        {
            var user = await _context.users.ToListAsync();
            return _mapper.Map<List<UserViewDto>>(user);
        }

        public async Task<UserViewDto> GetUserByID(int id)
        {
            var user = await _context.users.FindAsync(id);
            return _mapper.Map<UserViewDto>(user);
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            try
            {
                var isExists = await _context.products.FirstOrDefaultAsync(a => a.Id == id);
                if (isExists == null)
                {
                    return false;
                }

                _context.products.Remove(isExists);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddProduct(AddProductDto addPro, IFormFile image)
        {
            try
            {

                string imageUrl = await _cloudinary.UploadImageAsync(image);


                var category = await _context.categories
                    .FirstOrDefaultAsync(c => c.Id == addPro.CategoryId);

                if (category == null)
                {
                    throw new Exception("Invalid Category ID");
                }


                var product = new Product
                {
                    ProductName = addPro.ProductName,
                    ProductDescription = addPro.ProductDescription,

                    ProductPrice = addPro.ProductPrice,
                    offerPrize = addPro.OfferPrize,
                    Rating = addPro.Rating,
                    CategoryId = addPro.CategoryId,
                    ImageUrl = imageUrl,

                };


                await _context.products.AddAsync(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task UpdatePro(int id, UpdateProductDto Updtpro, IFormFile? image)
        {
            try
            {
                var pro = await _context.products.FirstOrDefaultAsync(x => x.Id == id);
                if (pro == null)
                {
                    throw new Exception($"Product with ID: {id} not found!");
                }

                if (Updtpro.CategoryId.HasValue)
                {
                    var CatExists = await _context.categories.FirstOrDefaultAsync(x => x.Id == Updtpro.CategoryId.Value);
                    if (CatExists == null)
                    {
                        throw new Exception("Category not found");
                    }
                    pro.CategoryId = Updtpro.CategoryId.Value;
                }

                if (!string.IsNullOrWhiteSpace(Updtpro.ProductName))
                {
                    pro.ProductName = Updtpro.ProductName;
                }

                if (!string.IsNullOrWhiteSpace(Updtpro.ProductDescription))
                {
                    pro.ProductDescription = Updtpro.ProductDescription;
                }

                if (Updtpro.ProductPrice.HasValue)
                {
                    pro.ProductPrice = Updtpro.ProductPrice.Value;
                }

                if (Updtpro.OfferPrize.HasValue)
                {
                    pro.offerPrize = Updtpro.OfferPrize.Value;
                }

                if (Updtpro.Rating.HasValue)
                {
                    pro.Rating = Updtpro.Rating.Value;
                }

                if (image != null && image.Length > 0)
                {
                    string imgUrl = await _cloudinary.UploadImageAsync(image);
                    pro.ImageUrl = imgUrl;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<OrderAdminViewDto>> GetOrderDetailsAdmin()
        {
            try
            {
                var orders = await _context.order
                    .Include(z => z._UserAd)
                    .Include(a => a.Items).ToListAsync();
                if (orders.Count > 0)
                {
                    var details = orders.Select(a => new OrderAdminViewDto
                    {
                        OrderId = a.Id,
                        OrderDate = a.OrderDate,
                        OrderString = a.OrderString,
                        OrderStatus = a.OrderStatus,
                        TransactionId = a.TransactionId,
                        UserName = a._UserAd.CustomerName,
                        Phone = a._UserAd.CustomerPhone,
                        UserAddress = a._UserAd.HomeAddress,


                    }).ToList();

                    return details;
                }

                return new List<OrderAdminViewDto>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<decimal> TotalRevenue()
        {
            try
            {
                var Total = await _context.orderItems.SumAsync(i => i.TotalPrice);
                return Convert.ToDecimal(Total);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> TotalProductsPurchased()
        {
            try
            {
                var total_pro = await _context.orderItems.SumAsync(i => i.Quantity);
                return Convert.ToInt32(total_pro);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<OrderViewDto>> GetOrderDetailsAdmin_byuserId(int userId)
        {
            try
            {
                var orders = await _context.order.Include(a => a.Items)
                   .ThenInclude(b => b.Product)
                   .Where(c => c.UserId == userId)
                   .ToListAsync();

                var orderDetails = orders.Select(a => new OrderViewDto
                {
                    OrderId = a.Id,
                    OrderDate = a.OrderDate,
                    OrderStatus = a.OrderStatus,
                    OrderString = a.OrderString,
                    TransactionId = a.TransactionId,
                    Items = a.Items.Select(b => new OrderItemDto
                    {
                        OrderItemId = b.OrderId,
                        OrderId = b._Order.Id,
                        ProductId = b.ProductId,
                        ProductImage = b.Product.ImageUrl,
                        ProductName = b.Product.ProductName,
                        Quantity = b.Quantity,
                        TotalPrice = b.TotalPrice,
                    }).ToList(),
                }).ToList();

                return orderDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> UpdateOrderStatus(int oId)
        {
            try
            {
                var order = await _context.order.FirstOrDefaultAsync(a => a.Id == oId);

                if (order == null)
                {
                    throw new Exception("Order not found");
                }

                //const string? OrderPlaced = "OrderPlaced";
                //const string? Delivered = "Delivered";


                order.OrderStatus = "Delivered";



                await _context.SaveChangesAsync();
                return order.OrderStatus;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message);
            }
        }


    }
}
