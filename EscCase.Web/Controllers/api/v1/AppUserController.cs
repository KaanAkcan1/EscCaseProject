using EscCase.Business.Interfaces;
using EscCase.Business.Models.Requests.AppUser;
using EscCase.Common.BaseControlle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscCase.Web.Controllers.api.v1
{
    [Route("/api/v1/auth")]
    public class AppUserController : BaseApiController
    {
        private readonly IAppUserService _appUserService;

        public AppUserController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        /// <summary>
        /// A controller used to login.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var loginResult = await _appUserService.LoginAsync(request);

            if (!loginResult.Success)
            {
                return Unauthorized(loginResult.Message);
            }

            return Ok(loginResult);
        }

        /// <summary>
        /// A controller used to validate token 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateToken(ValidateTokenRequest request)
        {
            var serviceResult = await _appUserService.ValidateTokenAsync(request.AccessToken, request.RefreshToken);

            if (!serviceResult.Success)
            {
                return Unauthorized(serviceResult.Message);
            }

            return Ok(serviceResult);
        }
    }
}
