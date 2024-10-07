using EscCase.Data.Contexts;
using EscCase.Data.Models;
using EscCase.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace EscCase.Business.Services
{
    public class JwtRefreshTokenService : Repository<JwtRefreshToken>
    {

        public JwtRefreshTokenService(ApplicationDbContext context) : base(context)
        {

        }

        private IConfigurationRoot appSettings
        {
            get
            {
                return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            }
        }

        public async Task RevokeAsync(Guid userId)
        {
            var tokensToRevoke = await FindAll(x => x.UserId == userId).Select(x => x.Id).ToListAsync();

            await DeleteAsync(tokensToRevoke, Guid.Empty, true);
        }

        public async Task DeleteExpiredTokensAsync()
        {
            var itemsToDelete = await FindAll(x => x.Expires < DateTime.UtcNow).Select(x => x.Id).ToListAsync();
            await DeleteAsync(itemsToDelete, Guid.Empty, true);
        }

        public async Task<string> CreateAsync(Guid userId, string accessToken)
        {
            await DeleteExpiredTokensAsync();

            var rt = Guid.NewGuid().ToString();

            var refreshToken = new JwtRefreshToken(userId, ComputeHash(rt), accessToken, Configuration.JwtOption.RefreshTokenExpirationMinutes);

            await CreateUpdateAsync(refreshToken);

            return rt;
        }

        public async Task<JwtRefreshToken?> GetByTokenAsync(string refreshToken)
        {
            await DeleteExpiredTokensAsync();

            var token = ComputeHash(refreshToken);

            return await FindAll(x => x.RefreshToken == token && x.Expires > DateTime.UtcNow).FirstOrDefaultAsync();
        }

        private enum HashAlgorithms
        {
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }

        private string ComputeHash(string value, HashAlgorithms hashAlgorithm = HashAlgorithms.SHA512)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(hashAlgorithm switch
            {
                HashAlgorithms.SHA1 => SHA1.Create().ComputeHash(bytes),
                HashAlgorithms.SHA256 => SHA256.Create().ComputeHash(bytes),
                HashAlgorithms.SHA384 => SHA384.Create().ComputeHash(bytes),
                HashAlgorithms.SHA512 => SHA512.Create().ComputeHash(bytes),
                _ => throw new NotImplementedException(),
            });
        }

        public string GenerateJwtToken(AppUser user, List<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(appSettings.GetSection("JwtSettings:AccessTokenSecret").Value ?? string.Empty);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string? ControlTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(appSettings.GetSection("JwtSettings:AccessTokenSecret").Value ?? string.Empty);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            if (jwtToken.ValidTo > DateTime.UtcNow)
                return token;
            else
                return null;
        }

        public Guid DecodeTokenAndGetUserIdAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(appSettings.GetSection("JwtSettings:AccessTokenSecret").Value ?? string.Empty);

            var result = Guid.Empty;

            try
            {
                var jwtObject = tokenHandler.ReadJwtToken(token);

                var jwtObjectPayload = jwtObject.Payload;

                if (jwtObjectPayload == null) return result;

                var nameIdPair = jwtObjectPayload.FirstOrDefault(x => x.Key == "nameid");

                if (nameIdPair.Value == null) return result;

                result = Guid.Parse(nameIdPair.Value.ToString() ?? Guid.Empty.ToString());

            }
            catch (Exception)
            {
                return result;
            }

            return result;
        }
    }
}
