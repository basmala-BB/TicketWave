using TicketWave.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TicketWave.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationdbContext _context;

        public CategoryController(ApplicationdbContext context)
        {
            _context = context;
        }

       
        public IActionResult Index()
        {
            var categories = _context.categories.AsNoTracking().ToList();
            return View(categories);
        }

   
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        } 

       [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

      
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.categories.Find(id);

            if (category == null)
                return RedirectToAction("NotFoundPage", "Home");

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.categories.Update(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

    
        public IActionResult Delete(int id)
        {
            var category = _context.categories.Find(id);

            if (category == null)
                return RedirectToAction("NotFoundPage", "Home");

            _context.categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}

