using EscCase.Business.Models.Requests.AppUser;
using EscCase.Common.Entities.Common;
using static EscCase.Business.Services.AppUserService;

namespace EscCase.Business.Interfaces
{
    public interface IAppUserService
    {
        Task<DataResponse<JwtToken>> LoginAsync(LoginRequest request);
        Task<DataResponse<JwtToken>> ValidateTokenAsync(string accessToken, string refreshToken);
    }
}
