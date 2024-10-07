using EscCase.Common.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace EscCase.Data.Models
{
    public class JwtRefreshToken : BaseEntity
    {
        public JwtRefreshToken() { }
        public JwtRefreshToken(Guid userId, string refreshToken, string accessToken, double expiresInMinutes)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            RefreshToken = refreshToken;
            AccessToken = accessToken;
            Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes);
            CreatedOn = DateTime.UtcNow;
        }


        public DateTime Expires { get; set; }

        public bool IsExpired => Expires < DateTime.UtcNow;

        [StringLength(1000)]
        public string RefreshToken { get; set; } = null!;

        public string AccessToken { get; set; } = null!;

        public Guid UserId { get; set; } = Guid.Empty!;

        public AppUser User { get; set; } = null!;
    }
}
