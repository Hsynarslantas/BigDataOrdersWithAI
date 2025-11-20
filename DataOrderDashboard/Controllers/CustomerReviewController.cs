using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.Controllers
{
    public class CustomerReviewController : Controller
    {
        private readonly BigDataOrderContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public CustomerReviewController(BigDataOrderContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
