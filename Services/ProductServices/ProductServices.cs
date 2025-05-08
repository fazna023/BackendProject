using AutoMapper;
using BackendProject2.Context;
using BackendProject2.Dto;
using Microsoft.EntityFrameworkCore;
using System.Drawing;




namespace BackendProject2.Services.ProductServices
{
    public class ProductServices:IProductServices  
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ProductServices(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<ProductWithCategoryDto>> HotDeals()
        {
            try
            {
                var productWithCategory = await _context.products
                    .Where(a => (a.ProductPrice - a.offerPrize) > 200).ToListAsync();

                return _mapper.Map<List<ProductWithCategoryDto>>(productWithCategory);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<List<ProductWithCategoryDto>> FeaturedPro()
        {
            try
            {
                var ProductWithCategory = await _context.products
                    .Where(c => c.Rating > 4).ToListAsync();

                return _mapper.Map<List<ProductWithCategoryDto>>(ProductWithCategory);

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message);
            }
        }


       

        public async Task<List<ProductWithCategoryDto>> GetProducts()
        {
            try
            {
                var productWithCategory = await _context.products
                    .Include(x=>x._Category)
                    .ToListAsync();
                if (productWithCategory == null)
                {
                    throw new Exception("Product is not exist");
                }

                return _mapper.Map<List<ProductWithCategoryDto>>(productWithCategory);
               

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<ProductWithCategoryDto> GetProductById(int id)
        {
            try
            {
             
                var product = await _context.products.FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return null;
                }

                return _mapper.Map<ProductWithCategoryDto>(product);




            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<List<ProductWithCategoryDto>> GetProductsByCategoryName(string Cat_name)
        {
            try
            {
                if (Cat_name.ToLower() == "all")
                {
                    var allpro = await _context.products.ToListAsync();
                    if (allpro == null)
                    {
                        return null;
                    }

                    var products = _mapper.Map<ProductWithCategoryDto>(allpro);


                 
                }

                var catP1 = await _context.products.Include(x=>x._Category)
                    .Where(b => b._Category.CategoryName.ToLower() == Cat_name.ToLower()).ToListAsync();
                    
                if (catP1 == null)
                {
                    return null;
                }

                return _mapper.Map<List<ProductWithCategoryDto>>(catP1);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


       


        public async Task<List<ProductWithCategoryDto>> SearchProduct(string search)
        {
            try
            {
                if (string.IsNullOrEmpty(search))
                {
                    return new List<ProductWithCategoryDto> { new ProductWithCategoryDto() };
                }

                var pro = await _context.products.Where(a => a.ProductName.ToLower().Contains(search.ToLower())).ToListAsync();

                return _mapper.Map<List<ProductWithCategoryDto>>(pro);
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public async Task<List<ProductWithCategoryDto>> ProductPagination(int page, int pageSize)
        {
            try
            {
                var products = await _context.products
                    .Include(p => p._Category)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return _mapper.Map<List<ProductWithCategoryDto>>(products);
            }
            catch (Exception)
            {
                throw; 
            }
        }


    }
}
