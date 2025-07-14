using Microsoft.AspNetCore.Mvc;

namespace ArtStation_Dashboard.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
