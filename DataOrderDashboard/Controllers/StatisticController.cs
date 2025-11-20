using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.Controllers
{
    public class StatisticController : Controller
    {
        private readonly BigDataOrderContext _context;

        public StatisticController(BigDataOrderContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.CategoryCount = _context.Categories.Count();
            ViewBag.CustomerCount = _context.Customers.Count();
            ViewBag.ProductCount = _context.Products.Count();
            ViewBag.OrderCount = _context.Orders.Count();
            ViewBag.CustomerCountryCount = _context.Customers.Select(x=>x.CustomerCountry).Distinct().Count();
            ViewBag.CustomerCityCount = _context.Customers.Select(x => x.CustomerCity).Distinct().Count();
            ViewBag.ProductStatusByCompleted = _context.Orders.Where(x => x.OrderStatus == "Teslim Edildi").Count();
            ViewBag.ProductStatusByReturn = _context.Orders.Where(x => x.OrderStatus == "İade Edildi").Count();
            ViewBag.SeptemberOrders = _context.Orders.Where(x => x.OrderDate >= new DateTime(2025, 9, 1)&&x.OrderDate<=new DateTime(2025,9,30)).Count();
            ViewBag.Orders2025Count= _context.Orders.Where(x => x.OrderDate >= new DateTime(2025, 1, 1) && x.OrderDate <= new DateTime(2025, 12, 31)).Count();
            ViewBag.PaymentMehtodByCrypto = _context.Orders.Where(x => x.PaymentMethod == "Kripto Ödeme").Count();
            ViewBag.OrderTypeByGift = _context.Orders.Where(x => x.OrderNotes == "Hediye paketi yapılsın").Count();
            return View();
        }
        public IActionResult TextualStatistic()
        {
            ViewBag.MostExpoensiveProduct=_context.Products.Where(x=>x.UnitPrice== (_context.Products.Max(y => y.UnitPrice))).Select(z=>z.ProductName).FirstOrDefault();
            ViewBag.MostCheapestProduct = _context.Products.Where(x => x.UnitPrice == (_context.Products.Min(y => y.UnitPrice))).Select(z => z.ProductName).FirstOrDefault();
            ViewBag.TopStockProduct=_context.Products.OrderByDescending(x=>x.StockQuantity).Take(1).Select(z=>z.ProductName).FirstOrDefault();
            ViewBag.LastAddedProduct=_context.Products.OrderByDescending(x=>x.ProductId).Take(1).Select(z=>z.ProductName).FirstOrDefault();
            ViewBag.LastAddedCustomer = _context.Customers.OrderByDescending(x => x.CustomerId).Take(1).Select(z => z.CustomerName + " " + z.CustomerSurname ).FirstOrDefault();
            ViewBag.MostPaymentMethodByName = _context.Orders.GroupBy(x => x.PaymentMethod).Select(y => new
            {
                PaymentMethod = y.Key,
                TotalOrders = y.Count(),
            }).OrderByDescending(z => z.TotalOrders).Select(t=>t.PaymentMethod).FirstOrDefault();

            ViewBag.MostOrderedProduct=_context.Orders.GroupBy(o=>o.Product.ProductName).Select(g=> new
            {
                ProductName = g.Key,
                TotalQuantity=g.Sum(o=>o.Quantity)
            }).OrderByDescending(x=>x.TotalQuantity).Select(y=>y.ProductName).FirstOrDefault();
            ViewBag.MinOrderedProduct= _context.Orders.GroupBy(o => o.Product.ProductName).Select(g => new
            {
                ProductName = g.Key,
                TotalQuantity = g.Sum(o => o.Quantity)
            }).OrderBy(x => x.TotalQuantity).Select(y => y.ProductName).FirstOrDefault();
            ViewBag.MaxOrderedCountry = _context.Orders.GroupBy(o => o.Customer.CustomerCountry).Select(g => new
            {
                Country = g.Key,
                TotalOrders = g.Count(),
            }).OrderByDescending(x => x.TotalOrders).Select(y => y.Country).FirstOrDefault();
            ViewBag.MaxOrderedCity = _context.Orders.GroupBy(o => o.Customer.CustomerCity).Select(g => new
            {
                City = g.Key,
                TotalOrders = g.Count(),
            }).OrderByDescending(x => x.TotalOrders).Select(y => y.City).FirstOrDefault();

            ViewBag.TopSalesByCategory=_context.Orders.GroupBy(o=>o.Product.Category.CategoryName).Select(g=> new
            {
                CategoryName = g.Key,
                TotalSales=g.Sum(x=>x.Quantity),
            }).OrderByDescending(x=>x.TotalSales).Select(y=>y.CategoryName).FirstOrDefault();
            ViewBag.MostOrderByCustomer=_context.Orders.GroupBy(o=> new { o.Customer.CustomerName, o.Customer.CustomerSurname }).Select(g=> new
            {
                FullName=g.Key.CustomerName + " " + g.Key.CustomerSurname,
                TotalOrders=g.Count(),
            }).OrderByDescending(x=>x.TotalOrders).Select(z=>z.FullName).FirstOrDefault();
            ViewBag.TopCompletedByProduct = _context.Orders.Where(x=>x.OrderStatus=="Teslim Edildi").GroupBy(o=>o.Product.ProductName).Select(g => new
            {
                ProductName = g.Key,
                TotalOrders = g.Count(),
            }).OrderByDescending(x => x.TotalOrders).Select(z => z.ProductName).FirstOrDefault();
            ViewBag.TopReturnedByProduct = _context.Orders.Where(x => x.OrderStatus == "İade Edildi").GroupBy(o => o.Product.ProductName).Select(g => new
            {
                ProductName = g.Key,
                TotalOrders = g.Count(),
            }).OrderByDescending(x => x.TotalOrders).Select(z => z.ProductName).FirstOrDefault();
            ViewBag.LowestSalesByCategory = _context.Orders.GroupBy(o => o.Product.Category.CategoryName).Select(g => new
            {
                CategoryName = g.Key,
                TotalSales = g.Sum(x => x.Quantity),
            }).OrderBy(x => x.TotalSales).Select(y => y.CategoryName).FirstOrDefault();
            ViewBag.TopSaledCountry = _context.Orders.GroupBy(o => o.Customer.CustomerCountry).Select(g => new
            {
                Country = g.Key,
                OrderCount = g.Count(),
            }).OrderByDescending(x => x.OrderCount).Select(y => y.Country).FirstOrDefault();
            return View();
        }
    }
}
