using BackendProject2.ApiResponse;
using BackendProject2.Dto;
using BackendProject2.Services.AddressServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendProject2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressServices _addressSrvices;
        public AddressController(IAddressServices addressServices)
        {
            _addressSrvices = addressServices;
        }

        [HttpPost("Add-new-Address")]
        [Authorize]
        public async Task<IActionResult> Add_newAdd([FromBody] AddNewAddressDto _dto)
        {
            try
            {
                var user_id = Convert.ToInt32(HttpContext.Items["Id"]);
                var res = await _addressSrvices.AddnewAddress(user_id, _dto);
                return Ok(new ApiResponse<string>(true, "Address added successfully.", "[done]", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("GetAddresses")]
        [Authorize]
        public async Task<IActionResult> GetAddresses()
        {
            try
            {
                var user_id = Convert.ToInt32(HttpContext.Items["Id"]);

                var res = await _addressSrvices.GetAddress(user_id);
                if (res == null || !res.Any())
                {
                    return NotFound(new ApiResponse<string>(true, "No addresses found for this user.", "[]", null));
                }

                return Ok(new ApiResponse<IEnumerable<GetAdressDto>>(true, "fetched ", res, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        [HttpDelete("delete-address/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> deleteAddres(int id)
        {
            try
            {
                var res = await _addressSrvices.RemoveAddress(id);
                if (!res)
                {
                    return NotFound(new ApiResponse<string>(false, "address not found", "[]", null));
                }

                return Ok(new ApiResponse<string>(true, "Address removed", "[]", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }




    }
}