using System.Threading.Tasks;
using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.ViewComponents.CustomerDetailViewComponents
{
    public class _CustomerDetailMainCoverComponentPartial:ViewComponent
    {
        private readonly   BigDataOrderContext _context;

        public _CustomerDetailMainCoverComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var values=await _context.Customers.Where(x=>x.CustomerId==id).FirstOrDefaultAsync();
            return View(values);
        }
    }
}
