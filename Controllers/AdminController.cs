using BackendProject2.ApiResponse;
using BackendProject2.Dto;
using BackendProject2.Services.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendProject2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _service;
        private readonly ILogger<ProductController> _logger;

        public AdminController(IAdminServices services, ILogger<ProductController> logger)
        {
            _service = services;
            _logger = logger;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetAllUser()
        {
            var user = await _service.GetUser();
            return Ok(user);
        }

        [HttpGet("{id}")]
        [Authorize(Roles ="Admin")]

        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _service.GetUserByID(id);
            if(user == null)
            {
              return  NotFound("User Not FOund");
            }
            return Ok(user);
        }

        [HttpDelete("id")]
        [Authorize(Roles ="Admin")]

        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _service.DeleteUser(id);
            if (user == null)
            {
                return NotFound("User Not Found");
            }
            return Ok("user Deleted succesfully");
        }

        [HttpPatch("toggle-Block")]
        [Authorize(Roles ="Admin")]

        public async Task<IActionResult> BlockOrUnblock(int id)
        {
            var result = await _service.BlockOrUnblock(id);
            return Ok(result);
        }



        [Authorize(Roles = "Admin")]
        [HttpPost("Add_Pro")]
        public async Task<IActionResult> AddPro([FromForm] AddProductDto new_pro, IFormFile image)
        {
            try
            {

                if (new_pro == null || image == null)
                {
                    return BadRequest("Invalid product data or image file.");
                }


                if (image.Length > 10485760)
                {
                    return BadRequest("File size exceeds the 10 MB limit.");
                }

                if (!image.ContentType.StartsWith("image/"))
                {
                    return BadRequest("Invalid file type. Only image files are allowed.");
                }

                await _service.AddProduct(new_pro, image);
                return Ok(new ApiResponse<string>(true, "Product added successfully!", null, null));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while adding a product.");


                return StatusCode(500, ex.InnerException?.Message);
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpPut("Update_Pro/{id}")]
        public async Task<IActionResult> Update_pro(int id, [FromForm] UpdateProductDto updateProduct_Dto, IFormFile? image)
        {
            try
            {
                await _service.UpdatePro(id, updateProduct_Dto, image);

                return Ok(new ApiResponse<string>(true, "product updated", null, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }






        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePro(int id)
        {
            try
            {
                bool res = await _service.DeleteProduct(id);
                if (res)
                {
                    return Ok(new ApiResponse<string>(true, "Product deleted", null, null));
                }

                return NotFound(new ApiResponse<string>(false, "Product Not found", null, null));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("get-order-details-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderDetailsAdmin()
        {
            try
            {
                var res = await _service.GetOrderDetailsAdmin();
                if (res.Count < 0)
                {
                    return BadRequest(new ApiResponse<string>(false, "no order found", null, null));
                }

                return Ok(new ApiResponse<IEnumerable<OrderAdminViewDto>>(true, " successfully.", res, null));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("Total-Revenue")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> TotalRevenue()
        {
            try
            {
                var res = await _service.TotalRevenue();
                return Ok(new ApiResponse<decimal>(true, " successfully.", res, null));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [HttpGet("Total-Products-Saled")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TotalProductsPurchased()
        {
            try
            {
                var res = await _service.TotalProductsPurchased();
                return Ok(new ApiResponse<int>(true, " successfully.", res, null));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("GetOrderDetailsAdmin_byuserId/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderDetailsAdmin_byuserId(int id)
        {
            try
            {
                var orderDetails = await _service.GetOrderDetailsAdmin_byuserId(id);
                if (orderDetails == null)
                {
                    return NotFound("User not found");
                }

                return Ok(new ApiResponse<IEnumerable<OrderViewDto>>(true, "done", orderDetails, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("update-order-status/{oid}")]
        public async Task<IActionResult> OrderStatusU(int oid)
        {
            try
            {
                var res = await _service.UpdateOrderStatus(oid);
                return Ok(new ApiResponse<string>(true, "updated", res, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
        }

    }
}
