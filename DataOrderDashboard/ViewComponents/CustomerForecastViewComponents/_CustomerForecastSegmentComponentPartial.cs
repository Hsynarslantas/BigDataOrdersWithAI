using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.ViewComponents.CustomerForecastViewComponents
{
    public class _CustomerForecastSegmentComponentPartial:ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _CustomerForecastSegmentComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public  IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
