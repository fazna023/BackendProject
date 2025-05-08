using AutoMapper;
using BackendProject2.ApiResponse;
using BackendProject2.Context;
using BackendProject2.Dto;
using BackendProject2.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendProject2.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CartService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CartWithTotalPrice> GetAllCartItems(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            var userCart = await _context.cart
                .Include(cart => cart._Items)
                .ThenInclude(item => item._Product)
                .FirstOrDefaultAsync(cart => cart.UserId == userId);

            if (userCart == null || userCart._Items == null || !userCart._Items.Any())
            {
                return new CartWithTotalPrice
                {
                    TotalCartPrice = 0,
                    c_items = new List<CartViewDto>()
                };
            }

            var cartItems = userCart._Items.Select(item => new CartViewDto
            {
                ProductId = item._Product.Id,
                ProductName = item._Product.ProductName,
                Price = (int?)item._Product.offerPrize,
                ProductImage = item._Product.ImageUrl,
                TotalAmount = Convert.ToInt32(item._Product.offerPrize) * item.ProductQty,
                OrginalPrize = Convert.ToInt32(item._Product.ProductPrice),
                Quantity = item.ProductQty
            }).ToList();

            var totalCartPrice = cartItems.Sum(item => (item.TotalAmount) ?? 0);


            return new CartWithTotalPrice
            {
                TotalCartPrice = Convert.ToInt32(totalCartPrice),
                c_items = cartItems
            };
        }



        public async Task<ApiResponse<CartViewDto>> AddToCart(int userId, int productId)
        {
            try
            {
                var user = await _context.users
                    .Include(a => a._Cart)
                    .ThenInclude(b => b._Items)
                    .FirstOrDefaultAsync(c => c.Id == userId);


                if (user == null)
                {

                    return new ApiResponse<CartViewDto>(false, "User not found!", null, "Check dtails");
                }


                if (user._Cart == null)
                {
                    var new_cart = new Cart
                    {
                        UserId = userId,
                    };

                    _context.cart.Add(new_cart);
                    await _context.SaveChangesAsync();

                    user._Cart = new_cart;
                }

                var check = user._Cart?._Items?.FirstOrDefault(a => a.ProductId == productId);
                if (check != null)
                {

                    return new ApiResponse<CartViewDto>(false, "Item alredy in your cart!", null, "Check dtails");


                }


                var pro = await _context.products.FirstOrDefaultAsync(a => a.Id == productId);
                if (pro == null || pro?.StockId <= 0)
                {

                    return new ApiResponse<CartViewDto>(false, "Product not found or out of stock.", null, "Check dtails");


                }


                var newItem = new CartItems
                {
                    ProductId = productId,
                    CartId = user._Cart.Id
                };


                _context.cartItems.Add(newItem);
                await _context.SaveChangesAsync();

                var res = new CartViewDto
                {
                    ProductId = productId,
                    cartviewid = user._Cart.Id
                };

                return new ApiResponse<CartViewDto>(true, "Successfully added to the cart", res, null);

            }



            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }



        public async Task<ApiResponse<string>> RemoveFromCart(int userId, int productId)
        {
            try
            {
                var user = await _context.users
                    .Include(a => a._Cart)
                    .ThenInclude(b => b._Items)
                    .FirstOrDefaultAsync(c => c.Id == userId);

                if (user == null)
                {
                    return new ApiResponse<string>(false, "User not found", null, "verify details");
                }

                var pro_check = user._Cart?._Items?.FirstOrDefault(a => a.ProductId == productId);
                if (pro_check == null)
                {
                    return new ApiResponse<string>(false, "Product not found in Cart", null, "Check your items");

                }

                _context.cartItems.Remove(pro_check);
                await _context.SaveChangesAsync();
                return new ApiResponse<string>(true, "Product removed from  Cart", "", null);

            }

            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while removing the item from the cart.", ex);
            }
        }



        public async Task<ApiResponse<CartViewDto>> IncraseQuantity(int userId, int productId)
        {
            try
            {

                var user = await _context.users
                    .Include(a => a._Cart)
                    .ThenInclude(b => b._Items)
                    .ThenInclude(c => c._Product)
                    .FirstOrDefaultAsync(c => c.Id == userId);

                if (user == null)
                {
                    return new ApiResponse<CartViewDto>(false, "user not found", null, "Check the informations");
                }



                var item = user._Cart?._Items?.FirstOrDefault(b => b.ProductId == productId);
                if (item == null)
                {

                    return new ApiResponse<CartViewDto>(false, "Product not found in cart", null, "Check the informations");

                }

                if (item.ProductQty >= 10)
                {

                    return new ApiResponse<CartViewDto>(false, "You reach max quantity (10)", null, "Check the informations");

                }

                if (item.ProductQty >= item._Product?.StockId)
                {

                    return new ApiResponse<CartViewDto>(false, "Out of stock", null, "Check the informations");

                }

                item.ProductQty++;
                await _context.SaveChangesAsync();
                var res = _mapper.Map<CartViewDto>(item);
                return new ApiResponse<CartViewDto>(true, "Quantity increased", res, null);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

        }



        public async Task<ApiResponse<CartViewDto>> DecreaseQuantity(int userId, int ProductId)
        {
            try
            {
                var user = await _context.users
                    .Include(a => a._Cart)
                    .ThenInclude(b => b._Items)
                    .ThenInclude(c => c._Product)
                    .FirstOrDefaultAsync(c => c.Id == userId);

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                var item = user?._Cart?._Items?.FirstOrDefault(b => b.ProductId == ProductId);
                if (item == null)
                {
                    return new ApiResponse<CartViewDto>(false, "Product not found", null, "Check the information provided");
                }

                if (item.ProductQty > 1)
                {

                    item.ProductQty--;
                }
                else
                {

                    user._Cart._Items.Remove(item);
                    item = null;
                }

                await _context.SaveChangesAsync();

                if (item == null)
                {
                    return new ApiResponse<CartViewDto>(true, "Item removed from cart", null, null);
                }

                var res = _mapper.Map<CartViewDto>(item);

                return new ApiResponse<CartViewDto>(true, "Quantity updated", res, null);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        public async Task<CartWithTotalPrice> GetAllCartByUser(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            var userCart = await _context.cart
                .Include(cart => cart._Items)
                .ThenInclude(item => item._Product)
                .FirstOrDefaultAsync(cart => cart.UserId == userId);

            if (userCart == null || userCart._Items == null || !userCart._Items.Any())
            {
                return new CartWithTotalPrice
                {
                    TotalCartPrice = 0,
                    c_items = new List<CartViewDto>()
                };
            }

            var cartItems = userCart._Items.Select(item => new CartViewDto
            {
                ProductId = item._Product.Id,
                ProductName = item._Product.ProductName,
                Price = (int?)item._Product.offerPrize,
                ProductImage = item._Product.ImageUrl,
                TotalAmount = Convert.ToInt32(item._Product.offerPrize) * item.ProductQty,
                OrginalPrize = Convert.ToInt32(item._Product.ProductPrice),
                Quantity = item.ProductQty
            }).ToList();

            var totalCartPrice = cartItems.Sum(item => (item.TotalAmount) ?? 0);

            return new CartWithTotalPrice
            {
                TotalCartPrice = Convert.ToInt32(totalCartPrice),
                c_items = cartItems
            };
        }



    }
}