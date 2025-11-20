using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.ViewComponents.CustomerForecastViewComponents
{
    public class _CustomerForecastMainStatisticsComponentPartial:ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _CustomerForecastMainStatisticsComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
           
            var totalCustomerCount = _context.Customers.Count();
            ViewBag.TotalCustomerCount = totalCustomerCount;
            var totalOrdersCount=_context.Orders.Count();
            var avgOrderByCustomerCount= totalOrdersCount / totalCustomerCount;
            ViewBag.AvgOrderByCustomerCount= avgOrderByCustomerCount;
            var AvgPrice=_context.Orders.Sum(o=>o.Quantity * o.Product.UnitPrice)/totalCustomerCount;
            ViewBag.AvgPrice=AvgPrice;
            var threeMontsAgo = DateTime.Now.AddMonths(-3);
            ViewBag.ActiveCustomerCount=_context.Orders.Where(x=>x.OrderDate>=threeMontsAgo).Select(x=>x.CustomerId).Distinct().Count();
            var sixMonthsAgo=DateTime.Now.AddMonths(-6);
            var inActiveCustomerCount = _context.Orders.Count(c => !_context.Orders.Any(o => o.CustomerId == c.CustomerId &&
            o.OrderDate >= sixMonthsAgo));
            ViewBag.InActiveCustomerCount= inActiveCustomerCount;
            

            return View();
        }
    }
}
