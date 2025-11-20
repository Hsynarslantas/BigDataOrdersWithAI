using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardChartComponentPartial:ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _DashboardChartComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            #region Main_Statistic3
            var firstDay = new DateTime(2025,11,01);
            var orderSales = _context.Orders.Where(x=>x.OrderStatus=="Teslim Edildi")
            .Where(o => o.OrderDate >= firstDay && o.OrderDate < firstDay.AddDays(1))
            .Sum(o => o.Quantity * o.Product.UnitPrice);
            ViewBag.OrderSales = Math.Round(orderSales, 2);

            var orderSalesPreparing = _context.Orders.Where(x => x.OrderStatus == "Hazırlanıyor")
          .Where(o => o.OrderDate >= firstDay && o.OrderDate < firstDay.AddDays(1))
          .Sum(o => o.Quantity * o.Product.UnitPrice);
            ViewBag.orderSalesPreparing = Math.Round(orderSalesPreparing, 2);

            var orderSalesShipped = _context.Orders.Where(x => x.OrderStatus == "Kargoya Verildi")
          .Where(o => o.OrderDate >= firstDay && o.OrderDate < firstDay.AddDays(1))
          .Sum(o => o.Quantity * o.Product.UnitPrice);
            ViewBag.orderSalesShipped = Math.Round(orderSalesShipped, 2);
            #endregion
            var sixMonthsAgo = DateTime.Today.AddMonths(-6);

            var monthlySalesRaw = _context.Orders
                .Where(o => o.OrderDate >= sixMonthsAgo)
                .GroupBy(o => new {
                    o.OrderDate.Year,
                    o.OrderDate.Month,
                    o.OrderStatus
                })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Durum = g.Key.OrderStatus,
                    SatisAdedi = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList(); 

       
            var monthlySales = monthlySalesRaw.Select(x => new
            {
                Ay = $"{x.Year}-{x.Month:D2}",
                x.Durum,
                x.SatisAdedi
            }).ToList();

   
            ViewBag.MonthlySalesJson = System.Text.Json.JsonSerializer.Serialize(monthlySales);
            return View();
        }
    }
}
