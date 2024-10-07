using EscCase.Business.Interfaces;
using EscCase.Business.Models.Requests;
using EscCase.Business.Models.Requests.Product;
using EscCase.Common.BaseControlle;
using Microsoft.AspNetCore.Mvc;

namespace EscCase.Web.Controllers.api.v1
{
    [Route("/api/v1/product")]
    public class ProductController : BaseApiController
    {
        private readonly IProductService _productService;
        public ProductController(
             IProductService productService
            )
        {
            _productService = productService;
        }

        /// <summary>
        /// A controller used for creating product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<IActionResult> Create(ProductCreateRequest request)
        {
            var serviceResult = await _productService.CreateFromCreateProductRequestAsync(request);

            if (!serviceResult.Success)
            {
                return BadRequest(serviceResult.Message);
            }

            return Ok(serviceResult);
        }


        /// <summary>
        ///  A controller used for updating product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("update")]
        public async Task<IActionResult> Update(ProductUpdateRequest request)
        {
            var serviceResult = await _productService.UpdateAsync(request);

            if (!serviceResult.Success)
            {
                return BadRequest(serviceResult.Message);
            }

            return Ok(serviceResult);
        }


        /// <summary>
        /// A controller used for deleting product from Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var serviceResult = await _productService.DeleteAsync(id);

            if (!serviceResult.Success)
            {
                return BadRequest(serviceResult.Message);
            }

            return Ok(serviceResult);
        }


        /// <summary>
        /// A controller used for creating products from json value
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("save-products")]
        public async Task<IActionResult> SaveProductsFromJson(SaveProductsFromJsonRequest request)
        {
            var serviceResult = await _productService.SaveProductsFromJsonAsync(request);

            if (!serviceResult.Success)
            {
                return BadRequest(serviceResult.Message);
            }

            return Ok();
        }


    }
}