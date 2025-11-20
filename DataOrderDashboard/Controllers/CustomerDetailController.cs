using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.Controllers
{
    public class CustomerDetailController : Controller
    {
        private readonly BigDataOrderContext _context;

        public CustomerDetailController(BigDataOrderContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            ViewBag.CustomerId = id;
            return View();
        }
    }
}
