using GreenActionPortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GreenActionPortal.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Home()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult CommunityInformation()
        {
            return View();
        }

        public IActionResult WasteManagement()
        {
            return View();
        }

        public IActionResult Activities()
        {
            return View();
        }
    }
}
