using DataOrderDashboard.Context;
using DataOrderDashboard.Models;
using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardTopCircleComponentPartial:ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _DashboardTopCircleComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var totalOrders = _context.Orders.Count();
            var preparingCount = _context.Orders.Where(x => x.OrderStatus == "Hazırlanıyor").Count();
            var successedCount = _context.Orders.Where(x => x.OrderStatus == "Teslim Edildi").Count();
            var shippedCount = _context.Orders.Where(x => x.OrderStatus == "Kargoya Verildi").Count();
            var canceledCount = _context.Orders.Where(x => x.OrderStatus == "İptal Edildi").Count();

            var result = new List<OrderStatusChartViewModel>
            {
                new OrderStatusChartViewModel
                {
                    Title = "Hazırlanıyor",
                    Percantage=totalOrders==0 ? 0 : (int)Math.Round(preparingCount * 100.0/totalOrders),
                    ChangeText="%6 Artış ⬆️",
                    IsPositive=true,
                    Color="#00BCD4",
                },
                 new OrderStatusChartViewModel
                {
                    Title = "Teslim Edildi",
                    Percantage=totalOrders==0 ? 0 : (int)Math.Round(successedCount * 100.0/totalOrders),
                    ChangeText="%10 Artış ⬆️",
                    IsPositive=true,
                    Color="#2196F3",
                },
                  new OrderStatusChartViewModel
                {
                    Title = "Kargoya Verildi",
                    Percantage=totalOrders==0 ? 0 : (int)Math.Round(shippedCount * 100.0/totalOrders),
                    ChangeText="%2 Artış ⬆️",
                    IsPositive=true,
                    Color="#FFFF00",
                },
                   new OrderStatusChartViewModel
                {
                    Title = "İptal Edildi",
                    Percantage=totalOrders==0 ? 0 : (int)Math.Round(canceledCount * 100.0/totalOrders),
                    ChangeText="%10 Azalış ⬇️",
                    IsPositive=false,
                    Color="#FF7043",
                }
            };
            


            return View(result);
        }
    }
}
