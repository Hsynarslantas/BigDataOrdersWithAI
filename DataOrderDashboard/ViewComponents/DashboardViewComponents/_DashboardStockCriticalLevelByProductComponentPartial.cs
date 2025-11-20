using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardStockCriticalLevelByProductComponentPartial:ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _DashboardStockCriticalLevelByProductComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values=_context.Products.OrderByDescending(x=>x.StockQuantity<10).Take(10).ToList();
            return View(values);
        }
    }
}
