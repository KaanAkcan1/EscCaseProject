using EscCase.Business.Interfaces;
using EscCase.Common.BaseController;
using EscCase.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace EscCase.Web.Controllers
{
    public class HomeController : BaseMvcController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var productList = await _productService.GetAllProductsAsync();

            return View(productList.Data);
        }

        /// <summary>
        /// A view controller used for creating Json view from all products data
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> FetchData()
        {
            var productList = await _productService.GetAllProductsAsync();

            if (!productList.Success || productList.Data == null)
            {
                return BadRequest();
            }

            return Json(productList.Data);
        }

        /// <summary>
        /// A view controller used for "creating random products and edit" view
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateData()
        {
            var serviceResult = _productService.CreateRandomProductList();

            if (!serviceResult.Success || serviceResult.Data == null)
            {
                return BadRequest();
            }

            ViewBag.ProductsJson = JsonSerializer.Serialize(serviceResult.Data);

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
