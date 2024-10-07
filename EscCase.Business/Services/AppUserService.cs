using EscCase.Business.Interfaces;
using EscCase.Business.Models.Requests.AppUser;
using EscCase.Common.Entities.App;
using EscCase.Common.Entities.Common;
using EscCase.Common.Enums.Common;
using EscCase.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;
using System.Text.Json;

namespace EscCase.Business.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly JwtRefreshTokenService _jwtRefreshTokenService;

        public AppUserService(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            JwtRefreshTokenService jwtRefreshTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtRefreshTokenService = jwtRefreshTokenService;
        }

        /// <summary>
        /// A function used for login process
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<DataResponse<JwtToken>> LoginAsync(LoginRequest request)
        {
            var result = new DataResponse<JwtToken>();

            var user = await _userManager.FindByNameAsync(request.Username) ?? await _userManager.FindByEmailAsync(request.Username);

            if (user == null || user.StatusId == (int)EntityStatus.Deleted)
            {
                if (request.Username == RepositoryDefaults.AdminLoginValues.UserName && request.Password == RepositoryDefaults.AdminLoginValues.Password)
                {
                    var processResult = await CreateAdminIfNotExistAsync();

                    if (processResult.Succeeded == false)
                    {
                        result.SetErrorCreate("Admin kullanıcısı oluşturulamadı !");

                        return result;
                    }

                    user = await _userManager.FindByNameAsync(RepositoryDefaults.AdminLoginValues.UserName);
                }
                else
                {
                    result.SetError("Kullanıcı bulunamadı ! ");

                    return result;
                }
            }

            var canSignInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!canSignInResult.Succeeded)
            {
                await _userManager.AccessFailedAsync(user);

                result.SetError("Kullanıcı adı şifre uyumsuz ! ");

                return result;
            }

            var tokenResult = await CreateJwtTokenAsync(user);


            if (tokenResult.Success)
            {
                result.SetSuccess(tokenResult.Data);
            }
            else
            {
                result.SetError("Bilgiler doğru ancak token oluşturulamadı !");
            }

            return result;
        }

        /// <summary>
        /// The function used for admin user creation
        /// </summary>
        /// <returns></returns>
        public async Task<IdentityResult> CreateAdminIfNotExistAsync()
        {
            var result = new DataResponse<AppUser>();

            await _roleManager.CreateAsync(new IdentityRole<Guid>()
            {
                Id = RepositoryDefaults.UserId.Administrator,
                Name = nameof(RepositoryDefaults.UserId.Administrator),
            });

            var user = new AppUser
            {
                Id = RepositoryDefaults.UserId.Administrator,
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                StatusId = RepositoryDefaults.DefaultEntityStatus,
                Email = RepositoryDefaults.AdminLoginValues.Email,
                UserName = RepositoryDefaults.AdminLoginValues.UserName,
                PhoneNumber = RepositoryDefaults.AdminLoginValues.PhoneNumber,
            };

            var adminCreateResult = await _userManager.CreateAsync(user, RepositoryDefaults.AdminLoginValues.Password);

            if (adminCreateResult.Succeeded == false)
            {
                return adminCreateResult;
            }

            var adminRoleCreateResult = await _userManager.AddToRoleAsync(user, nameof(RepositoryDefaults.UserId.Administrator));

            return adminRoleCreateResult;
        }

        /// <summary>
        /// A function used for token refresh
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<DataResponse<JwtToken>> RefreshTokenAsync(string refreshToken)
        {
            var result = new DataResponse<JwtToken>();

            var token = await _jwtRefreshTokenService.GetByTokenAsync(refreshToken);

            if (token == null)
            {
                result.SetError("Token bulunamadı !");

                return result;
            }

            var user = await _userManager.FindByIdAsync(token.UserId.ToString());

            if (user == null || user.StatusId != (int)EntityStatus.Active)
            {
                result.SetError("Bu Id'ye sahip aktif kullanıcı bulunamadı !");

                return result;
            }

            return await CreateJwtTokenAsync(user);

        }

        public record JwtToken(string AccessToken, string RefreshToken, long ExpiresIn);

        /// <summary>
        /// A function used to create jwt token 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<DataResponse<JwtToken>> CreateJwtTokenAsync(AppUser user)
        {
            var result = new DataResponse<JwtToken>();

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var recordedClaims = await _userManager.GetClaimsAsync(user);

            var appClaims = new List<AppClaim>();

            foreach (var claim in recordedClaims)
            {
                var appClaim = new AppClaim()
                {
                    UserId = user.Id.ToString(),
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                };

                appClaims.Add(appClaim);
            }

            var newClaims = new List<Claim>()
            {
                new Claim("appClaims", JsonSerializer.Serialize(appClaims)),
            };

            claims.AddRange(newClaims);

            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));

            var token = _jwtRefreshTokenService.GenerateJwtToken(user, claims);

            var refreshToken = await _jwtRefreshTokenService.CreateAsync(user.Id, token);

            result.Data = new JwtToken(token, refreshToken, (long)Configuration.JwtOption.AccessTokenExpirationMinutes * 60);

            result.Success = true;

            return result;

        }


        /// <summary>
        /// A function used to validate token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<DataResponse<JwtToken>> ValidateTokenAsync(string accessToken, string refreshToken)
        {
            var result = new DataResponse<JwtToken>();

            var tokenRecord = await _jwtRefreshTokenService.GetByTokenAsync(refreshToken);

            if (tokenRecord == null)
                return result;

            var tokenUserId = _jwtRefreshTokenService.DecodeTokenAndGetUserIdAsync(accessToken);

            if (tokenUserId == Guid.Empty || tokenRecord.AccessToken != accessToken || tokenRecord.UserId != tokenUserId)
                return result;

            result.Success = true;
            result.Data = new JwtToken(tokenRecord.AccessToken, refreshToken, (long)Configuration.JwtOption.AccessTokenExpirationMinutes * 60);

            return result;
        }

    }
}
