using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardLast5ReviewsComponentPartial:ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _DashboardLast5ReviewsComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values=_context.Reviews.OrderByDescending(x=>x.ReviewId).Include(y=>y.Product).Include(z=>z.Customer).Take(5).ToList();
            return View(values);
        }
    }
}
