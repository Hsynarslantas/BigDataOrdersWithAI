using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.ViewComponents.CustomerForecastViewComponents
{
    public class _CustomerForecastHeaderComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
