using BackendProject2.ApiResponse;
using BackendProject2.Dto;

namespace BackendProject2.Services.WishListServices
{
    public interface IWishListServices
    {
        Task<ApiResponse<string>> AddOrRemove(int user_id, int Pro_id);
        Task<ApiResponse<string>> RemoveFromWishList(int user_id, int pro_id);
        Task<List<WishListViewDto>> GetAllWishItems(int user_id);
    }
}
