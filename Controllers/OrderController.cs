using BackendProject2.ApiResponse;
using BackendProject2.Dto;
using BackendProject2.Services.OrderServices;
using BackendProject2.Services.OrdersServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;

namespace BackendProject2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly IOrderServices _orderServices;
        public OrderController(IOrderServices orderServices)
        {
            _orderServices = orderServices;
        }




        [Authorize]
        [HttpPost("individual-pro-buy/{pro_id}")]
        public async Task<IActionResult> individual_probuy(int pro_id, CreateOrderDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Order details are required.");
                }

                var user_id = Convert.ToInt32(HttpContext.Items["Id"]);
                var res = await _orderServices.Indidvidual_ProductBuy(user_id, pro_id, dto);

                //return Ok("Product purchased successfully.");
                return Ok(new ApiResponse<string>(true, "Product purchased successfully.", "done", null));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Place-order")]
        [Authorize]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderDto createOrder_Dto)
        {
            try
            {
                var user_id = Convert.ToInt32(HttpContext.Items["Id"]);
                var res = await _orderServices.CrateOrder_CheckOut(user_id, createOrder_Dto);
                //return Ok(res + "Order placed");
                return Ok(new ApiResponse<string>(true, " successfully.", "done", null));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetOrder-Details")]
        public async Task<IActionResult> GetorderDetails()
        {
            try
            {
                var u_id = Convert.ToInt32(HttpContext.Items["Id"]);
                var res = await _orderServices.GetOrderDetails(u_id);

                return Ok(new ApiResponse<IEnumerable<OrderViewDto>>(true, " successfully.", res, null));


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost("razor-order-create")]
        public async Task<IActionResult> RazorOrderCreate([FromQuery] long price)
        {
            try
            {
                if (price <= 0)
                {
                    return BadRequest("Enter a valid amount.");
                }

                var orderId = await _orderServices.RazorOrderCreate(price);
                return Ok(new ApiResponse<string>(true, "Order created", orderId, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


            [Authorize]
            [HttpPost("razor-payment-verify")]

            public async Task<IActionResult> RazorPaymentVerify([FromBody] PaymentDto razorpay)
            {
                try
                {
                    if (razorpay == null)
                    {
                        return BadRequest(new ApiResponse<string>(false, "razorpay details must not null here", null, null));
                    }
                    var res = await _orderServices.RazorPayment(razorpay);
                    if (!res)
                    {
                        return BadRequest(new ApiResponse<string>(false, "Error in payment", "", "check payment details"));
                    }
                    return Ok(new ApiResponse<string>(true, "done", "Success", null));
                }
                catch (Exception ex)
                {
                    return Ok(ex.InnerException?.Message);
                }
            }

        } 

        }