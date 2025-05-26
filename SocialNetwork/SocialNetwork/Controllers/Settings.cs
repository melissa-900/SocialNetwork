using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Controllers
{
    public class Settings : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
