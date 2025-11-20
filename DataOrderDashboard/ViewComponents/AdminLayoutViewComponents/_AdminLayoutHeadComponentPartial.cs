using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.ViewComponents.AdminLayoutViewComponents
{
    public class _AdminLayoutHeadComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
