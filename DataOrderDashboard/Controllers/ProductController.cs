using DataOrderDashboard.Context;
using DataOrderDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.Controllers
{
    public class ProductController : Controller
    {
        private readonly BigDataOrderContext _context;

        public ProductController(BigDataOrderContext context)
        {
            _context = context;
        }

        public IActionResult ProductList(int page=1)
        {
            int pageSize = 15;
            var values = _context.Products.OrderBy(x=>x.ProductId).Skip((page - 1)* pageSize).Take(pageSize).Include(y=>y.Category).ToList();
            int totalCount=_context.Products.Count();
            ViewBag.TotalPages=(int)Math.Ceiling(totalCount/(double)pageSize);
            ViewBag.CurrentPage = page;
            return View(values);
        }
        [HttpGet]
        public IActionResult CreateProduct()
        {
            var categoryList = _context.Categories.Select(x => new SelectListItem
            {
                Text = x.CategoryName,
                Value = x.CategoryId.ToString(),
            }).ToList();
            ViewBag.CategoryList = categoryList;
            return View();
        }
        [HttpPost]
        public IActionResult CreateProduct(Product Product)
        {
            var values = _context.Products.Add(Product);
            _context.SaveChanges();
            return View(values);
        }
        public IActionResult DeleteProduct(int id)
        {
            var values = _context.Products.Find(id);
            _context.Products.Remove(values);
            _context.SaveChanges();
            return RedirectToAction("ProductList");
        }
        [HttpGet]
        public IActionResult UpdateProduct(int id)
        {
            var categoryList = _context.Categories.Select(x => new SelectListItem
            {
                Text = x.CategoryName,
                Value = x.CategoryId.ToString(),
            }).ToList();
            ViewBag.CategoryList = categoryList;
            var values = _context.Products.Find(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult UpdateProduct(Product Product)
        {
            var values = _context.Products.Update(Product);
            _context.SaveChanges();
            return RedirectToAction("ProductList");
        }
    }
}
