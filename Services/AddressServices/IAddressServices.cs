
using BackendProject2.Dto;

namespace BackendProject2.Services.AddressServices
{
    public interface IAddressServices
    {
        Task<bool> AddnewAddress(int userId, AddNewAddressDto address);
        Task<List<GetAdressDto>> GetAddress(int userId);
        Task<bool> RemoveAddress(int addId);

    }
}