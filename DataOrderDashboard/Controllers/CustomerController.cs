using DataOrderDashboard.Context;
using DataOrderDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataOrderDashboard.Controllers
{
    public class CustomerController : Controller
    {
        private readonly BigDataOrderContext _context;

        public CustomerController(BigDataOrderContext context)
        {
            _context = context;
        }

        public IActionResult CustomerList(int page = 1)
        {
            int pageSize = 15;
            var values = _context.Customers.OrderBy(x => x.CustomerId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            int totalCount = _context.Customers.Count();
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.CurrentPage = page;
            return View(values);
        }
        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateCustomer(Customer Customer)
        {
            var values = _context.Customers.Add(Customer);
            _context.SaveChanges();
            return RedirectToAction("CustomerList");
        }
        public IActionResult DeleteCustomer(int id)
        {
            var values = _context.Customers.Find(id);
            _context.Customers.Remove(values);
            _context.SaveChanges();
            return RedirectToAction("CustomerList");
        }
        [HttpGet]
        public IActionResult UpdateCustomer(int id)
        {
            var values = _context.Customers.Find(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult UpdateCustomer(Customer Customer)
        {
            var values = _context.Customers.Update(Customer);
            _context.SaveChanges();
            return RedirectToAction("CustomerList");
        }
        public IActionResult GetCustomer(int id)
        {
            var values = _context.Customers.Find(id);
            return View(values);
        }
    }
}
