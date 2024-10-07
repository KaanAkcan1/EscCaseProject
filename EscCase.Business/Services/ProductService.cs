using EscCase.Business.Interfaces;
using EscCase.Business.Models.Requests;
using EscCase.Business.Models.Requests.Product;
using EscCase.Business.Models.Responses.Product;
using EscCase.Common.Entities.Common;
using EscCase.Common.Enums.Common;
using EscCase.Data.Models;
using EscCase.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EscCase.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWorkRepository;


        public ProductService(IUnitOfWork unitOfWorkRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        
        public async Task<DataResponse<List<Product>>> GetAllProductsAsync()
        {
            var result = new DataResponse<List<Product>>();

            var repositoryResult = await _unitOfWorkRepository.productRepository.FindAll(x => x.StatusId == (int)EntityStatus.Active).ToListAsync();

            if (repositoryResult == null || repositoryResult.Count == 0)
            {

                result.SetError("There is no data for given request !");

                return result;
            }

            result.SetSuccess(repositoryResult);

            return result;
        }


        public DataResponse<List<ProductWithoutSensetiveData>> CreateRandomProductList()
        {
            var result = new DataResponse<List<ProductWithoutSensetiveData>>();

            var productList = new List<ProductWithoutSensetiveData>();

            var random = new Random();

            for (var i = 0; i < 5; i++)
            {
                var randomProduct = new ProductWithoutSensetiveData
                {
                    ProductCode = "P" + random.Next(1, 1000).ToString("D3"),
                    ProductName = "Product " + random.Next(1, 100),
                    Quantity = random.Next(1, 500),
                    UnitPrice = Math.Round(random.NextDouble() * 100, 2)
                };

                productList.Add(randomProduct);
            }

            result.SetSuccesCreate(productList);

            return result;
        }

        //Adını değiştir
        public async Task<DataResponse<List<Product>>> CreateAsync(List<Product> products)
        {
            var result = new DataResponse<List<Product>>();

            var repositoryResult = await _unitOfWorkRepository.productRepository.CreateAsync(products);

            if (repositoryResult == null || repositoryResult.Success == false || repositoryResult.Data == null)
            {
                result.SetErrorCreate(repositoryResult == null ? "Liste kaydedilemedi !" : repositoryResult.Message);

                return result;
            }

            result.SetSuccesCreate(repositoryResult.Data);

            return result;
        }


        public async Task<DataResponse<Product>> CreateFromCreateProductRequestAsync(ProductCreateRequest request)
        {
            var result = new DataResponse<Product>();

            var products = new List<Product>();

            var product = new Product()
            {
                Code = request.ProductCode,
                Name = request.ProductName,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice
            };

            products.Add(product);

            var serviceResult = await CreateAsync(products);

            if (!serviceResult.Success)
            {
                result.SetErrorCreate(serviceResult.Message);
            }
            else
            {
                result.SetSuccesCreate(serviceResult.Data == null ? product : serviceResult.Data.FirstOrDefault());
            }

            return result;
        }


        public async Task<Response> SaveProductsFromJsonAsync (SaveProductsFromJsonRequest request)
        {
            var result = new Response();

            try
            {
                var products = JsonSerializer.Deserialize<List<Product>>(request.UpdatedProductsJson);

                if (products == null)
                {
                    result.SetError("Gönderilen ürün listesi boş !");

                    return result;
                }

                var serviceResult = await CreateAsync(products);

                if (serviceResult.Data == null || !serviceResult.Success)
                {

                    result.SetError("Gönderilen ürün listesi kaydedilemedi !");

                    return result;
                }
            }
            catch (Exception)
            {
                result.SetError("Gönderilen ürün listesi json formatına uygun değil !");

                return result;
            }

            result.SetSuccess();

            return result;
        }


        public async Task<DataResponse<Product>> UpdateAsync(ProductUpdateRequest request)
        {
            var result = new DataResponse<Product>();

            var product = _unitOfWorkRepository.productRepository.FindAll(x => x.Id == request.Id && x.StatusId == (int)EntityStatus.Active).FirstOrDefault();

            if (product == null)
            {
                result.SetErrorCreate("Ürün Id'si yanlış geldi!" );

                return result;
            }

            product.Code = request.ProductCode;
            product.Name = request.ProductName;
            product.Quantity = request.Quantity;
            product.UnitPrice = request.UnitPrice;

            var repositoryResult = await _unitOfWorkRepository.productRepository.CreateUpdateAsync(product);

            if (repositoryResult == null || repositoryResult.Success == false || repositoryResult.Data == null)
            {
                result.SetErrorCreate(repositoryResult == null ? "Değişiklikler kaydedilemedi !" : repositoryResult.Message);

                return result;
            }

            result.SetSuccesCreate(repositoryResult.Data.FirstOrDefault());

            return result;
        }


        public async Task<DataResponse<Product>> DeleteAsync(Product product)
        {
            var result = new DataResponse<Product>();

            var repositoryResult = await _unitOfWorkRepository.productRepository.DeleteAsync(product.Id, RepositoryDefaults.UserId.Administrator);

            if (repositoryResult == null || repositoryResult.Success == false || repositoryResult.Data == null)
            {
                result.SetErrorCreate(repositoryResult == null ? "Veri silinemedi !" : repositoryResult.Message);

                return result;
            }

            result.SetSuccess(repositoryResult.Data.FirstOrDefault());

            return result;
        }


        public async Task<Response> DeleteAsync(Guid id)
        {
            var result = new Response();

            var repositoryResult = await _unitOfWorkRepository.productRepository.DeleteAsync(id, RepositoryDefaults.UserId.Administrator);

            if (repositoryResult == null || repositoryResult.Success == false || repositoryResult.Data == null)
            {
                result.SetError(repositoryResult == null ? "Veri silinemedi !" : repositoryResult.Message);

                return result;
            }

            result.SetSuccess();

            return result;
        }
    }
}
