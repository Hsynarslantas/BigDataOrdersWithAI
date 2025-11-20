using DataOrderDashboard.Context;
using DataOrderDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardTodayOrdersComponentPartial:ViewComponent
    {
        private readonly  BigDataOrderContext _context;

        public _DashboardTodayOrdersComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var lastOrderDate = _context.Orders
               .OrderByDescending(o => o.OrderDate)
               .Select(o => o.OrderDate.Date)
               .FirstOrDefault();
            var last5orders=_context.Orders.Include(x=>x.Customer)
                .Include(y=>y.Product).Where(z=>z.OrderDate.Date==lastOrderDate)
                .OrderByDescending(t => t.OrderDate).Take(5).Select(g => new OrderSummaryViewModel
                {
                   OrderId= g.OrderId,
                   ProductName=  g.Product.ProductName,
                   CustomerName=g.Customer.CustomerName + " " + g.Customer.CustomerSurname,
                   Quantity= g.Quantity,
                   PaymentMethod= g.PaymentMethod,
                   OrderStatus= g.OrderStatus,
                   UnitPrice=g.Product.UnitPrice,
                }).ToList();
            return View(last5orders);
        }
    }
}
