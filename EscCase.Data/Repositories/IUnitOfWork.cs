using EscCase.Data.Repositories.Abstract;

namespace EscCase.Data.Repositories
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        public IAppUserRepository appUserRepository { get; }
        public IJwtRefreshTokenRepository jwtRefreshTokenRepository { get; }
        public IProductRepository productRepository { get; }

    }
}
