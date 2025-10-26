using GEEKS.Dto;
using GEEKS.Utils;
using GEEKS.Models;

namespace GEEKS.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<dynamic>> Register(RegisterDTO registerRequest);
        Task<ServiceResponse<dynamic>> Login(LoginDTO loginRequest);
        Task<User> FindUserByEmail(string email);
        Task UpdatePassword(User user, string newPassword);
    }
}
