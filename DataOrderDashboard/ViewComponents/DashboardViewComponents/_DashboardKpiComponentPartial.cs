using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardKpiComponentPartial:ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _DashboardKpiComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            #region İstatistik1
            var ordersDay = new DateTime(2025, 11, 1);
            var yesterday = ordersDay.AddDays(-1);

            var ordersDayCount=_context.Orders.Where(x=>x.OrderDate==ordersDay).Count();
            var yesterdayOrderCount=_context.Orders.Where(x=>x.OrderDate==yesterday).Count();   
            
            if (ordersDayCount > yesterdayOrderCount)
            {
                ViewBag.Icon = "zmdi zmdi-trending-up float-right";
            }
            else
            {
                ViewBag.Icon = "zmdi zmdi-trending-down float-right";
            }
            decimal changeRate = 0;
            if (yesterdayOrderCount > 0)
            {
                changeRate = ((decimal)(ordersDayCount - yesterdayOrderCount)/yesterdayOrderCount) * 100;
            }
            var dailyAvgOrders = _context.Orders.GroupBy(x => x.OrderDate.Date).Select(g => g.Count()).Average();
            double ratio = 0;
            if (dailyAvgOrders > 0)
            {
                ratio=(ordersDayCount/dailyAvgOrders) * 100.0;
                
            }
            ViewBag.FirstDayVsDailyAvgOrders = dailyAvgOrders;
            ViewBag.FirstDay = ordersDayCount;
            ViewBag.OrderChange = Math.Round(changeRate,2);
            #endregion
            #region İstatistik2
            var firstDay = new DateTime(2025,11,01);
            var sevenDaysAgo=firstDay.AddDays(-7);
            var totalOrder7Day = _context.Orders.Count(x => x.OrderDate >= sevenDaysAgo && x.OrderDate < firstDay.AddDays(1));
            var cancelledOrders7Days = _context.Orders.Count(x => x.OrderStatus == "İptal Edildi" && x.OrderDate >= sevenDaysAgo && x.OrderDate < firstDay.AddDays(1));
            decimal cancelRate = 0;
            cancelRate = ((decimal)cancelledOrders7Days / totalOrder7Day) * 100;
            ViewBag.CancelRate = Math.Round(cancelRate,2);
            ViewBag.CancelColor = "red";
            ViewBag.CancelText = cancelRate > 5 ? "Yüksek İptal Etme Oranı ⚠️" : "Normal Düzeyde";

            #endregion
            #region İstatistik3
            var totalOrders=_context.Orders.Count();
            var completedOrders = _context.Orders.Count(x => x.OrderStatus == "Teslim Edildi");

            decimal completedRate = 0;
            if(completedOrders > 0)
            {
                completedRate= ((decimal)completedOrders / totalOrders)* 100;

            }
            ViewBag.CompletedOrders= completedOrders;
            ViewBag.CompletionRate=Math.Round(completedRate,2);
            ViewBag.CompletionColor = "green";
            ViewBag.CompletionTrend = "Artış ⬆️";
            ViewBag.CompletionText = completedRate > 80 ? "Mükemmel Perfomans 💪 " : "Yükseliş Devam Ediyor 📈";


            #endregion
            return View();
        }
    }
}
