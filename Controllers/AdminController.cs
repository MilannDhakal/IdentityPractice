using Microsoft.AspNetCore.Mvc;
namespace IdentityPractice.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
