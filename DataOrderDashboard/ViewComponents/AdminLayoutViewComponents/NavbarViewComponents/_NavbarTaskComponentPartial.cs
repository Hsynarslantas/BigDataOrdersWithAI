using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.ViewComponents.AdminLayoutViewComponents.NavbarViewComponents
{
    public class _NavbarTaskComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
