using EscCase.Data.Contexts;
using EscCase.Data.Repositories.Abstract;
using EscCase.Data.Repositories.Concrate;

namespace EscCase.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;
        private IAppUserRepository _appUserRepository;
        private IJwtRefreshTokenRepository _jwtRefreshTokenRepository;
        private IProductRepository _productRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public IAppUserRepository appUserRepository => _appUserRepository ?? new AppUserRepository();
        public IJwtRefreshTokenRepository jwtRefreshTokenRepository => _jwtRefreshTokenRepository ?? new JwtRefreshTokenRepository(_context);
        public IProductRepository productRepository => _productRepository ?? new ProductRepository(_context);


        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
