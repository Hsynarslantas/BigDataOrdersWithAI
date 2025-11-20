using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.ViewComponents.AdminLayoutViewComponents
{
    public class _AdminLayoutPageLoaderComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
