using DataOrderDashboard.Context;
using DataOrderDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.Controllers
{
    public class ReviewController : Controller
    {
        private readonly BigDataOrderContext _context;

        public ReviewController(BigDataOrderContext context)
        {
            _context = context;
        }

        public IActionResult ReviewList(int page = 1)
        {
            int pageSize = 15;
            var values = _context.Reviews.OrderBy(x => x.ReviewId).Skip((page - 1) * pageSize).Take(pageSize).Include(y => y.Product).Include(z=>z.Customer).ToList();
            int totalCount = _context.Reviews.Count();
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.CurrentPage = page;
            return View(values);
        }
        [HttpGet]
        public IActionResult CreateReview()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateReview(Review Review)
        {
            var values = _context.Reviews.Add(Review);
            _context.SaveChanges();
            return View(values);
        }
        public IActionResult DeleteReview(int id)
        {
            var values = _context.Reviews.Find(id);
            _context.Reviews.Remove(values);
            _context.SaveChanges();
            return RedirectToAction("ReviewList");
        }
        [HttpGet]
        public IActionResult UpdateReview(int id)
        {
            var values = _context.Reviews.Find(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult UpdateReview(Review Review)
        {
            var values = _context.Reviews.Update(Review);
            _context.SaveChanges();
            return RedirectToAction("ReviewList");
        }
    }
}
