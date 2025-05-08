using BackendProject2.Dto;
using BackendProject2.ApiResponse;

namespace BackendProject2.Services.AdminServices
{
    public interface IAdminServices
    {
        Task<ApiResponse<bool>> BlockOrUnblock(int id);

        Task<List<UserViewDto>> GetUser();

        Task<UserViewDto> GetUserByID(int id);
        Task<bool> DeleteUser(int id);

        Task AddProduct(AddProductDto addpro, IFormFile image);
        Task<bool> DeleteProduct(int id);
        Task UpdatePro(int id, UpdateProductDto updtpro, IFormFile image);

        Task<List<OrderAdminViewDto>> GetOrderDetailsAdmin();
        Task<decimal> TotalRevenue();
        Task<int> TotalProductsPurchased();
        Task<List<OrderViewDto>> GetOrderDetailsAdmin_byuserId(int userId);
        Task<string> UpdateOrderStatus(int oId);

    }
}
