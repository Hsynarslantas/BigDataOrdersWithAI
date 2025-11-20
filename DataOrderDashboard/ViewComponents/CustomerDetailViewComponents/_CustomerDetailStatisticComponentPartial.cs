using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.ViewComponents.CustomerDetailViewComponents
{
    public class _CustomerDetailStatisticComponentPartial:ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _CustomerDetailStatisticComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public async Task <IViewComponentResult>InvokeAsync(int id)
        {
            ViewBag.TotalOrderCount = await _context.Orders.Where(x => x.CustomerId == id).CountAsync();

            ViewBag.TotalCompletedOrderCount = await _context.Orders.Where(x => x.CustomerId == id && x.OrderStatus == "Teslim Edildi").CountAsync();

            ViewBag.TotalCanceledOrderCount = await _context.Orders.Where(x => x.CustomerId == id && x.OrderStatus == "İptal Edildi").CountAsync();

            ViewBag.GetCustomerIdByCountry = await
            _context.Customers
            .Where(x => x.CustomerId == id)
            .Select(y => y.CustomerCountry)
            .FirstOrDefaultAsync() ?? "Belirtilmemiş";

            ViewBag.GetCustomerIdByCity = await
            _context.Customers
            .Where(x => x.CustomerId == id)
            .Select(y => y.CustomerCity)
            .FirstOrDefaultAsync() ?? "Belirtilmemiş";

             ViewBag.TotalSpentForCustomer = await _context.Orders
            .Where(o => o.CustomerId == id)
            .Include(o => o.Product)
            .SumAsync(o => o.Product.UnitPrice * o.Quantity);

            return View();
            
        }
    }
}
