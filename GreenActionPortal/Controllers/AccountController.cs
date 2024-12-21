using Microsoft.AspNetCore.Mvc;

namespace GreenActionPortal.Controllers
{
    public class AccountController : BaseController
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
