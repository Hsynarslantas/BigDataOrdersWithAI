using DataOrderDashboard.Context;
using DataOrderDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.Controllers
{
    public class OrderController : Controller
    {
        private readonly BigDataOrderContext _context;

        public OrderController(BigDataOrderContext context)
        {
            _context = context;
        }

        public IActionResult OrderList(int page = 1)
        {
            int pageSize = 15;
            var values = _context.Orders.OrderBy(x => x.OrderId).Skip((page - 1) * pageSize).Take(pageSize).Include(y=>y.Customer).Include(z=>z.Product).ToList();
            int totalCount = _context.Orders.Count();
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.CurrentPage = page;
            return View(values);
        }
            [HttpGet]
            public IActionResult CreateOrder()
            {
                var customerList = _context.Customers.Select(x => new SelectListItem
                {
                    Text = x.CustomerName,
                    Value = x.CustomerId.ToString(),
                }).ToList();
                ViewBag.CustomerList = customerList;
                var productList = _context.Products.Select(x => new SelectListItem
                {
                    Text = x.ProductName,
                    Value = x.ProductId.ToString(),
                }).ToList();
                ViewBag.ProductList = productList;
                return View();
            }
            [HttpPost]
            public IActionResult CreateOrder(Order Order)
            {
                var values = _context.Orders.Add(Order);
                _context.SaveChanges();
                return RedirectToAction("OrderList");
            }
        public IActionResult DeleteOrder(int id)
        {
            var values = _context.Orders.Find(id);
            _context.Orders.Remove(values);
            _context.SaveChanges();
            return RedirectToAction("OrderList");
        }
        [HttpGet]
        public IActionResult UpdateOrder(int id)
        {

            var customerList = _context.Customers.Select(x => new SelectListItem
            {
                Text = x.CustomerName,
                Value = x.CustomerId.ToString(),
            }).ToList();
            ViewBag.CustomerList = customerList;
            var productList = _context.Products.Select(x => new SelectListItem
            {
                Text = x.ProductName,
                Value = x.ProductId.ToString(),
            }).ToList();
            ViewBag.ProductList = productList;
            var values = _context.Orders.Find(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult UpdateOrder(Order Order)
        {
            var values = _context.Orders.Update(Order);
            _context.SaveChanges();
            return RedirectToAction("OrderList");
        }
    }
}
