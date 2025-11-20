using System.Security.Cryptography;
using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.ViewComponents.CustomerForecastViewComponents
{
    public class _CustomerForecastCategoryWChartComponentPartial:ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _CustomerForecastCategoryWChartComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var firstDate = new DateTime(2025, 11, 1);
            var TopCategoriesFirstDate = _context.Orders.Include(o => o.Product).ThenInclude(p => p.Category).Where(x => x.OrderDate.Date == firstDate)
                .AsEnumerable().GroupBy(o => o.Product.Category.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    OrderCount = g.Count()
                }).OrderByDescending(x => x.OrderCount).Take(3).ToList();
            if (TopCategoriesFirstDate.Count > 0)
            {
                ViewBag.TopCategory1Name = TopCategoriesFirstDate[0].CategoryName;
                ViewBag.TopCategory1Count= TopCategoriesFirstDate[0].OrderCount;
            }
            if (TopCategoriesFirstDate.Count > 1)
            {
                ViewBag.TopCategory2Name = TopCategoriesFirstDate[1].CategoryName;
                ViewBag.TopCategory2Count = TopCategoriesFirstDate[1].OrderCount;
            }
            if (TopCategoriesFirstDate.Count > 2)
            {
                ViewBag.TopCategory3Name = TopCategoriesFirstDate[2].CategoryName;
                ViewBag.TopCategory3Count = TopCategoriesFirstDate[2].OrderCount;
            }
            #region Chart
            var categoryData =_context.Orders.Include(o=>o.Product).ThenInclude(p=>p.Category)
                .AsEnumerable().GroupBy(o => new
                {
                    Year=o.OrderDate.Year,
                    CategoryName=o.Product.Category.CategoryName,

                }).Select(g => new
                {
                     Year=g.Key.Year,
                     CategoryName=g.Key.CategoryName,
                     OrderCount=g.Count()
                }).OrderBy(x=>x.Year).ToList(); 

            //Her Bir Kategorinin Toplam Satış Sayısı
            var topCategories=categoryData.
                GroupBy(x=>x.CategoryName)
                .Select(g => new
                {
                    CategoryName=g.Key,
                    TotalOrders=g.Sum(x=>x.OrderCount)
                }).OrderByDescending(x=>x.TotalOrders).Take(5).Select(x=>x.CategoryName).ToList();

            //Sadece bu 5 Kategoriye ait verilerin listesi
            var filteredData=categoryData.Where(x=>topCategories.Contains(x.CategoryName)).ToList();
            var years=filteredData.Select(x=>x.Year).Distinct().OrderBy(x=>x).ToList();

            var chartSeries = topCategories.Select(category => new
            {
                name=category,
                data=years.Select(y=>filteredData.FirstOrDefault(cd=>cd.CategoryName==category&& cd.Year==y)?.OrderCount??0).ToList()
            }).ToList();

            ViewBag.Years=years;
            ViewBag.Series=chartSeries;
            #endregion
            return View();
        }
    }
}
