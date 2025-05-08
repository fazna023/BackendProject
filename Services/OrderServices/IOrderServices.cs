using BackendProject2.Dto;

namespace BackendProject2.Services.OrderServices
{
    public interface IOrderServices
    {
        Task<bool> CrateOrder_CheckOut(int userId, CreateOrderDto createOrderDto);
        Task<bool> Indidvidual_ProductBuy(int userId, int productId, CreateOrderDto order_Dto);
        Task<List<OrderViewDto>> GetOrderDetails(int userId);

        Task<string> RazorOrderCreate(long price);
        Task<bool> RazorPayment(PaymentDto payment);



    }
}
