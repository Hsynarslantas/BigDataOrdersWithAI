using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace DataOrderDashboard.ViewComponents.CustomerDetailViewComponents
{
    public class _CustomerDetailProfileDetailComponentPartial:ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _CustomerDetailProfileDetailComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public async Task< IViewComponentResult> InvokeAsync(int id)
        {
            var values= await _context.Customers.Where(x=>x.CustomerId== id).FirstOrDefaultAsync();
            return View(values);
        }
    }
}
