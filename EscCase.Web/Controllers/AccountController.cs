using EscCase.Business.Interfaces;
using EscCase.Business.Models.Requests.AppUser;
using Microsoft.AspNetCore.Mvc;

namespace EscCase.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAppUserService _appUserService;
        public AccountController(
             IAppUserService appUserService
            )
        {
            _appUserService = appUserService;
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
