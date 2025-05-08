using BackendProject2.Context;
using BackendProject2.Dto;
using BackendProject2.Models;
using BackendProject2.Services.OrderServices;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api; 
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BackendProject2.Services.OrdersServices
{
    public class OrderServices : IOrderServices
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public OrderServices(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        //razor pay

        //public async Task<string> RazorPayOrderCreate(long price)
        //{
        //    try
        //    {
        //        Dictionary<string, object> input = new Dictionary<string, object>();
        //        Random random = new Random();
        //        string TrasactionId = random.Next(0, 1000).ToString();
        //        input.Add("amount", Convert.ToDecimal(price) * 100);
        //        input.Add("currency", "INR");
        //        input.Add("receipt", TrasactionId);

        //        string key = _configuration["Razorpay:KeyId"];
        //        string secret = _configuration["Razorpay:KeySecret"];

        //        RazorpayClient client = new RazorpayClient(key, secret);
        //        Razorpay.Api.Order order = client.Order.Create(input);
        //        var OrderId = order["id"].ToString();

        //        return OrderId;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

       

        public async Task<bool> Indidvidual_ProductBuy(int userId, int productId, CreateOrderDto order_Dto)
        {
            try
            {
                var pro = await _context.products.FirstOrDefaultAsync(a => a.Id == productId);
                if (pro == null)
                {
                    throw new Exception("Product not found");
                }

                if (pro.StockId <= 0)
                {
                    throw new Exception("Product is out of stock");
                }

                var order = await _context.order
               .Include(a => a.Items)
               .ThenInclude(b => b.Product)
               .FirstOrDefaultAsync(c => c.UserId == userId);

                var new_order = new Models.Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    AddressId = order_Dto.AddId,
                    Total = order_Dto.Total ?? 0,
                    OrderString = order_Dto.OrderString,
                    TransactionId = order_Dto.TransactionId,
                    Items = new List<OrderItems>()
                };

                var orderItem = new OrderItems
                {
                    ProductId = pro.Id,
                    Quantity = 1,
                    TotalPrice = pro.offerPrize * 1
                };

                new_order.Items?.Add(orderItem);
                _context.order.Add(new_order);

                pro.StockId -= 1;
                _context.products.Update(pro);

                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while saving changes: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public async Task<bool> CrateOrder_CheckOut(int userId, CreateOrderDto createOrderDto)
        {
            try
            {
                var cart = await _context.cart
                    .Include(b => b._Items)
                    .ThenInclude(c => c._Product)
                    .FirstOrDefaultAsync(z => z.UserId == userId);

                if (cart == null || cart._Items == null || !cart._Items.Any())
                {
                    throw new Exception("Cart is empty");
                }

                decimal total = cart._Items.Sum(item => item._Product.offerPrize * item.ProductQty);
                string orderString = string.Join(", ", cart._Items.Select(item => item._Product.ProductName));
                string transactionId = $"TXN{DateTime.Now.Ticks}";

                var orderItems = cart._Items.Select(a => new OrderItems
                {
                    ProductId = a._Product.Id,
                    Quantity = a.ProductQty,
                    TotalPrice = a._Product.offerPrize * a.ProductQty
                }).ToList();

                var orderTotal = orderItems.Sum(x => x.TotalPrice);

                var order = new Models.Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    AddressId = createOrderDto.AddId,
                    Total = orderTotal,
                    OrderString = orderString,
                    TransactionId = transactionId,

                    Items = orderItems
                };

                foreach (var item in cart._Items)
                {
                    var pro = await _context.products.FirstOrDefaultAsync(a => a.Id == item.ProductId);
                    if (pro != null)
                    {
                        if (pro.StockId < item.ProductQty)
                        {
                            return false;
                        }

                        pro.StockId -= item.ProductQty;
                    }
                }

                await _context.order.AddAsync(order);
                _context.cart.Remove(cart);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Order creation failed: " + ex.Message, ex);
            }
        }

        public async Task<List<OrderViewDto>> GetOrderDetails(int userId)
        {
            try
            {
                var orders = await _context.order
                    .Include(a => a.Items)
                    .ThenInclude(b => b.Product)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                var orderDetails = orders
                    .Where(r => r.Items?.Count > 0)
                    .Select(a => new OrderViewDto
                    {
                        OrderId = a.Id,
                        OrderDate = a.OrderDate,
                        OrderStatus = a.OrderStatus,
                        OrderString = a.OrderString,
                        TransactionId = a.TransactionId,
                        Items = a.Items.Select(b => new OrderItemDto
                        {
                            OrderItemId = b.OrderId,
                            OrderId = b.OrderId,
                            ProductId = b.ProductId,
                            ProductName = b.Product.ProductName,
                            Quantity = b.Quantity,
                            TotalPrice = b.TotalPrice,
                            ProductImage = b.Product.ImageUrl  // ✅ Correct assignment
                        }).ToList(),
                    }).ToList();

                return orderDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //razor pay paynent verfication
        //razor pay

        public async Task<string> RazorOrderCreate(long price)
        {
            try
            {
                Dictionary<string, object> input = new Dictionary<string, object>();
                Random random = new Random();
                string TrasactionId = random.Next(0, 1000).ToString();
                input.Add("amount", Convert.ToDecimal(price) * 100);
                input.Add("currency", "INR");
                input.Add("receipt", TrasactionId);

                string key = _configuration["Razorpay:Key"];
                string secret = _configuration["Razorpay:Secret"];

                RazorpayClient client = new RazorpayClient(key, secret);
                Razorpay.Api.Order order = client.Order.Create(input);
                var OrderId = order["id"].ToString();

                return OrderId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //razor pay paynent verfication

        //razor pay paynent verfication
        public async Task<bool> RazorPayment(PaymentDto payment)
        {
            if (payment == null ||
               string.IsNullOrEmpty(payment.razorpay_payment_id) ||
               string.IsNullOrEmpty(payment.razorpay_order_id) ||
               string.IsNullOrEmpty(payment.razorpay_signature))
            {
                return false;
            }
            try
            {
                RazorpayClient client = new RazorpayClient(
                   _configuration["Razorpay:Key"],
                   _configuration["Razorpay:Secret"]
               );

                Dictionary<string, string> attributes = new Dictionary<string, string>
                {
                    { "razorpay_payment_id", payment.razorpay_payment_id },
                    { "razorpay_order_id", payment.razorpay_order_id },
                    { "razorpay_signature", payment.razorpay_signature }
                };

                Utils.verifyPaymentSignature(attributes);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message);
            }
        }





    }
};
