using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
