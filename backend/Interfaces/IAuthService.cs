using GEEKS.Dto;
using GEEKS.Utils;

namespace GEEKS.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<dynamic>> Register(RegisterDTO registerRequest);
        Task<ServiceResponse<dynamic>> Login(LoginDTO loginRequest);
    }
}
