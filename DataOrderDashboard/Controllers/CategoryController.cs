using DataOrderDashboard.Context;
using DataOrderDashboard.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DataOrderDashboard.Controllers
{
    public class CategoryController : Controller
    {
        private readonly BigDataOrderContext _context;

        public CategoryController(BigDataOrderContext context)
        {
            _context = context;
        }

        public IActionResult CategoryList()
        {
            var values=_context.Categories.ToList();
            return View(values);
        }
        [HttpGet]
        public IActionResult CreateCategory()
        {
            var values = _context.Categories.ToList();
            return View(values);
        }
        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            var values = _context.Categories.Add(category);
            _context.SaveChanges();
            return View(values);
        }
        public IActionResult DeleteCategory(int id)
        {
            var values = _context.Categories.Find(id);
            _context.Categories.Remove(values);
            _context.SaveChanges();
            return RedirectToAction("CategoryList");
        }
        [HttpGet]
        public IActionResult UpdateCategory(int id)
        {
            var values = _context.Categories.Find(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult UpdateCategory(Category category)
        {
            var values = _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction("CategoryList");
        }
    }
}
