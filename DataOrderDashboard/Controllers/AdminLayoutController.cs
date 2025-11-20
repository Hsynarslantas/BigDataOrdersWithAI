using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.Controllers
{
    public class AdminLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
