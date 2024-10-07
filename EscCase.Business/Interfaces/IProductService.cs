using EscCase.Business.Models.Requests;
using EscCase.Business.Models.Requests.Product;
using EscCase.Business.Models.Responses.Product;
using EscCase.Common.Entities.Common;
using EscCase.Data.Models;

namespace EscCase.Business.Interfaces
{
    public interface IProductService
    {
        Task<DataResponse<List<Product>>> GetAllProductsAsync();

        DataResponse<List<ProductWithoutSensetiveData>> CreateRandomProductList();

        Task<DataResponse<List<Product>>> CreateAsync(List<Product> products);

        Task<DataResponse<Product>> CreateFromCreateProductRequestAsync(ProductCreateRequest request);

        Task<Response> SaveProductsFromJsonAsync(SaveProductsFromJsonRequest request);

        Task<DataResponse<Product>> UpdateAsync(ProductUpdateRequest request);

        Task<DataResponse<Product>> DeleteAsync(Product product);

        Task<Response> DeleteAsync(Guid id);
    }
}
