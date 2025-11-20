using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardSubStatisticComponentPartial:ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _DashboardSubStatisticComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.CategoryCount = _context.Categories.Count();
            ViewBag.ProductCount = _context.Products.Count();
            ViewBag.CustomerCount = _context.Customers.Count();
            ViewBag.OrderCount = _context.Orders.Count();
            ViewBag.TotalRevenue = _context.Orders.Include(x => x.Product).Where(x => x.OrderStatus == "Teslim Edildi").
                Sum(o => o.Quantity * o.Product.UnitPrice);
            ViewBag.AvgBasketOrder=_context.Orders.Include(x=>x.Product).Where(x=>x.OrderStatus=="Teslim Edildi").Average(o=>o.Quantity * o.Product.UnitPrice);
            return View();
        }
    }
}
