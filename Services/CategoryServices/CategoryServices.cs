using AutoMapper;
using BackendProject2.ApiResponse;
using BackendProject2.Context;
using BackendProject2.Dto;
using BackendProject2.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendProject2.Services.CategoryServices
{
    public class CategoryServices : ICategoryServices
    {

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoryServices(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CategoryDto>> GetCategories()
        {
            var c = await _context.categories.ToListAsync();

            var res = _mapper.Map<List<CategoryDto>>(c);

            return res;
        }


        public async Task<ApiResponse<CatAddDto>> AddCategory(CatAddDto categorty)
        {
            var isExists = await _context.categories.FirstOrDefaultAsync(a => a.CategoryName == categorty.CategoryName);

            if (isExists != null)
            {
                return new ApiResponse<CatAddDto>(false, "category already exists", null, "add another category");
            }

            var res = _mapper.Map<Category>(categorty);
            _context.categories.Add(res);
            await _context.SaveChangesAsync();
            var respose = _mapper.Map<CatAddDto>(res);

            return new ApiResponse<CatAddDto>(true, "new category added to database", respose, null);
        }


        public async Task<ApiResponse<string>> RemoveCategory(int id)
        {
            try
            {
                var res = await _context.categories.FirstOrDefaultAsync(a => a.Id == id);
                var pro = await _context.products.Where(a => a.CategoryId == id).ToListAsync();

                if (res == null)
                {
                    return new ApiResponse<string>(false, "category not found", "", "check the de");
                }
                else
                {
                    _context.products.RemoveRange(pro);
                    _context.categories.Remove(res);
                    await _context.SaveChangesAsync();
                    return new ApiResponse<string>(true, "done", "category deleted", null);
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while saving chages: {ex.InnerException?.Message ?? ex.Message}");

            }
        }

    }
}