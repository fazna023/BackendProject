using BackendProject2.Dto;

namespace BackendProject2.Services.ProductServices
{
    public interface IProductServices
    {
     
        Task<List<ProductWithCategoryDto>> GetProducts();
        Task<List<ProductWithCategoryDto>> FeaturedPro();
        Task<ProductWithCategoryDto> GetProductById(int id);
        Task<List<ProductWithCategoryDto>> GetProductsByCategoryName(string categoryname);
      
        Task<List<ProductWithCategoryDto>> SearchProduct(string search);
        Task<List<ProductWithCategoryDto>> HotDeals();

        Task<List<ProductWithCategoryDto>> ProductPagination(int page, int pagesize);
    }
}
