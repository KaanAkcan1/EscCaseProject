using EscCase.Data.Contexts;
using EscCase.Data.Models;
using EscCase.Data.Repositories.Abstract;

namespace EscCase.Data.Repositories.Concrate
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
