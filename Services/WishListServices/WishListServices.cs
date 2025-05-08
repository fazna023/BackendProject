using BackendProject2.ApiResponse;
using BackendProject2.Context;
using BackendProject2.Dto;
using BackendProject2.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendProject2.Services.WishListServices
{
    public class WishListServices:IWishListServices
    {
        private readonly AppDbContext _context;
        public WishListServices(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<string>> AddOrRemove(int user_id, int pro_id)
        {
            try
            {
                var isExists = await _context.whishlist
                    .Include(a => a._Product)
                    .FirstOrDefaultAsync(b => b.ProductId == pro_id && b.UserId == user_id);

                if (isExists == null)
                {
                    var add_wish = new WishList
                    {
                        UserId = user_id,
                        ProductId = pro_id,
                    };

                    _context.whishlist.Add(add_wish);
                    await _context.SaveChangesAsync();
                    return new ApiResponse<string>(true, "Item added to the wishList", "done", null);
                }
                else
                {
                    _context.whishlist.Remove(isExists);
                    await _context.SaveChangesAsync();

                    return new ApiResponse<string>(true, "Item removed from wishList", "done", null);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while updating the wishlist.", ex);
            }
        }

        public async Task<List<WishListViewDto>> GetAllWishItems(int user_id)
        {
            try
            {
                var items = await _context.whishlist
                    .Include(a => a._Product)
                    .ThenInclude(b => b._Category)
                    .Where(c => c.UserId == user_id)
                    .ToListAsync();

                if (items != null)
                {
                    var p = items.Select(a => new WishListViewDto
                    {
                        Id = a.Id,
                        ProductId = a._Product.Id,
                        ProductName = a._Product.ProductName,
                        ProductDescription = a._Product.ProductDescription,
                        Price = a._Product.ProductPrice,
                        OfferPrice = a._Product.offerPrize,
                        ProductImage = a._Product.ImageUrl,
                        CategoryName = a._Product._Category?.CategoryName
                    }).ToList();

                    return p;
                }
                else
                {
                    return new List<WishListViewDto>();
                }

            }

            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }


        public async Task<ApiResponse<string>> RemoveFromWishList(int user_id, int pro_id)
        {
            try
            {
                var wishlistItem = await _context.whishlist
                    .Include(a => a._Product)
                    .FirstOrDefaultAsync(b => b.ProductId == pro_id && b.UserId == user_id);

                if (wishlistItem != null)
                {
                    _context.whishlist.Remove(wishlistItem);
                    await _context.SaveChangesAsync();

                    return new ApiResponse<string>(true, "Item removed from wishlist", "done", null);
                }

                return new ApiResponse<string>(false, "Product not found in wishlistt", "", null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, "An error occurred while removing the item from the wishlist", ex.Message, null);
            }
        }




    }
}
