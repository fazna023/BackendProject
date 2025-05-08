using BackendProject2.ApiResponse;
using BackendProject2.Dto;

using BackendProject2.Models;

namespace BackendProject2.Services.CategoryServices
{
    public interface ICategoryServices
    {

        Task<List<CategoryDto>> GetCategories();
        Task<ApiResponse<CatAddDto>> AddCategory(CatAddDto categorty);

        Task<ApiResponse<string>> RemoveCategory(int id);

    }
}