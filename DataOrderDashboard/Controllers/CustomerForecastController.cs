using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;

namespace DataOrderDashboard.Controllers
{
    public class CustomerForecastController : Controller
    {
        private readonly BigDataOrderContext _context;
        private readonly MLContext _mlContext;

        public CustomerForecastController(BigDataOrderContext context, MLContext mlContext)
        {
            _context = context;
            _mlContext = mlContext;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
