using FilmPass.Data;
using FilmPass.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmPass.Areas.Admin.Controllers
{ 
    [Area ("Admin")]
    public class CategoryController : Controller
    {
        ApplicationdbContext _context = new();
        public IActionResult Index()
        {
          var categories = _context.categories.AsNoTracking().AsQueryable();


            return View(categories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            _context.categories.Add(category);
            _context.SaveChanges();

            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.categories.FirstOrDefault(e => e.Id == id);

            if (category is null)
                return RedirectToAction("NotFoundPage", "Home");

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            _context.categories.Update(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var category = _context.categories.FirstOrDefault(e => e.Id == id);

            if (category is null)
                return RedirectToAction("NotFoundPage", "Home");

            _context.categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
