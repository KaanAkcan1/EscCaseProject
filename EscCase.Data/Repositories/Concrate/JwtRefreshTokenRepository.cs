using EscCase.Data.Contexts;
using EscCase.Data.Models;
using EscCase.Data.Repositories.Abstract;

namespace EscCase.Data.Repositories.Concrate
{
    public class JwtRefreshTokenRepository : Repository<JwtRefreshToken>, IJwtRefreshTokenRepository
    {
        public JwtRefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
