using BackendProject2.ApiResponse;
using BackendProject2.Dto;
using BackendProject2.Services.ProductServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendProject2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _Services;
        

        public ProductController(IProductServices services)
        {
            _Services = services;
          
        }

        [HttpGet("HotDeals")]
        public async Task<IActionResult> HotDeals()
        {
            try
            {
                var res = await _Services.HotDeals();
                return Ok(new ApiResponse<IEnumerable<ProductWithCategoryDto>>(true, "Product fetched", res, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FeaturedPro")]
        public async Task<IActionResult> FeturedPro()
        {
            try
            {
                var res = await _Services.FeaturedPro();
                return Ok(new ApiResponse<IEnumerable<ProductWithCategoryDto>>(true, "Product fetched", res, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _Services.GetProducts();
                return Ok(new ApiResponse<IEnumerable<ProductWithCategoryDto>>(true, "Products fetched ", products, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var p = await _Services.GetProductById(id);

                if (p == null)
                {
                    return Ok(new ApiResponse<string>(false, "Product not found", " ", null));
                }
                return Ok(new ApiResponse<ProductWithCategoryDto>(true, "Product  found", p, null));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("getByCategoryName")]
        public async Task<IActionResult> GetByCateName(string CatName)
        {
            try
            {
                var p = await _Services.GetProductsByCategoryName(CatName);
                if (p == null)
                {
                    return Ok(new ApiResponse<string>(false, "No products in this category", " ", null));
                }
                return Ok(new ApiResponse<IEnumerable<ProductWithCategoryDto>>(true, " products in this category", p, null));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


       

        [HttpGet("search-item")]
        public async Task<IActionResult> SearchPro(string search)
        {
            try
            {
                var res = await _Services.SearchProduct(search);
                if (res == null)
                {
                    return NotFound(new ApiResponse<string>(true, "no products matched", null, null));

                }
                return Ok(new ApiResponse<IEnumerable<ProductWithCategoryDto>>(true, " products are match with..", res, null));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<List<ProductWithCategoryDto>>> ProductPagination([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Page and pageSize must be greater than zero.");

            var result = await _Services.ProductPagination(page, pageSize);
            return Ok(result);
        }


    }
}