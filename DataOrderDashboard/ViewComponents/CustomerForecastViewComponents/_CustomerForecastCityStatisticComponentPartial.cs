using DataOrderDashboard.Context;
using DataOrderDashboard.Models.ForecastModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.ViewComponents.CustomerForecastViewComponents
{
    public class _CustomerForecastCityStatisticComponentPartial:ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _CustomerForecastCityStatisticComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var data=_context.Orders.Include(o => o.Customer).
                GroupBy(x => x.Customer.CustomerCity).Select(g => new CityOrderViewModel
                {
                    City=g.Key,
                    OrderCount=g.Count()

                }).OrderByDescending(x=>x.OrderCount).Take(6).ToList();

            var totalOrders=data.Sum(x=>x.OrderCount);

            foreach (var item in data)
            {
                item.Percentage = Math.Round((double)item.OrderCount / totalOrders * 100, 1);
            }

            return View(data);
        }
    }
}
