using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardHeaderComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();  
        }
    }
}
