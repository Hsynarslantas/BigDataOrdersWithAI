using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.ViewComponents.CustomerForecastViewComponents
{
    public class _CustomerForecastStatisticsComponentPartial : ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _CustomerForecastStatisticsComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var CustomerCount=_context.Customers.Count();
            var TotalOrderCount = _context.Orders.Count();
            
            var avgQuantity = _context.Orders.Average(o => o.Quantity);
      
             var startDate = new DateTime(2025, 10, 30); 
            var octLastDayOrderCount = _context.Orders.Where(x => x.OrderDate.Date == startDate).Count();
            ViewBag.TotalOrderC = TotalOrderCount;
            ViewBag.AverageQuantity = avgQuantity;
            ViewBag.CustomerCount=CustomerCount;
            ViewBag.OctoberLastDayOrderCount= octLastDayOrderCount;
            return View();
        }
    }
}
