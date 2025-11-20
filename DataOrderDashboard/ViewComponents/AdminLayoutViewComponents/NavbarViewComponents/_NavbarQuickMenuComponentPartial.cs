using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.ViewComponents.AdminLayoutViewComponents.NavbarViewComponents
{
    public class _NavbarQuickMenuComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
