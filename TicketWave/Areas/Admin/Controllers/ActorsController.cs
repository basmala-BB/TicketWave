
using TicketWave.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TicketWave.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorsController : Controller
    {
        private readonly ApplicationdbContext _context = new();

        // ================= Index =================
        public IActionResult Index()
        {
            var actors = _context.actors.AsNoTracking().ToList();
            return View(actors);
        }

        // ================= Create GET =================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ActorVM());
        }

        // ================= Create POST =================
        [HttpPost]
        public IActionResult Create(ActorVM model)
        {
            if (!ModelState.IsValid) return View(model);

            string fileName = "";
            if (model.Image != null && model.Image.Length > 0)
            {
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                fileName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
                var filePath = Path.Combine(imagesFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                model.Image.CopyTo(stream);
            }

            _context.actors.Add(new Actors { Name = model.Name, ImagePath = fileName });
            _context.SaveChanges();

            TempData["Notification"] = "Actor added successfully!";
            return RedirectToAction("Index");
        }
        // ================= Edit GET =================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var actor = _context.actors.Find(id);
            if (actor == null) return NotFound();

            var vm = new ActorVM { Name = actor.Name };
            ViewBag.ActorId = actor.Id;
            ViewBag.ImagePath = actor.ImagePath;

            return View(vm);
        }

        // ================= Edit POST =================
        [HttpPost]
        public IActionResult Edit(int id, ActorVM model)
        {
            var actor = _context.actors.Find(id);
            if (actor == null) return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            actor.Name = model.Name;

            if (model.Image != null && model.Image.Length > 0)
            {
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(Path.GetFileName(model.Image.FileName));
                var filePath = Path.Combine(imagesFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                model.Image.CopyTo(stream);

                if (!string.IsNullOrEmpty(actor.ImagePath))
                {
                    var oldPath = Path.Combine(imagesFolder, actor.ImagePath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                actor.ImagePath = fileName;
            }

            _context.actors.Update(actor);
            _context.SaveChanges();

            TempData["Notification"] = "Actor updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ================= Delete =================
        public IActionResult Delete(int id)
        {
            var actor = _context.actors.Find(id);
            if (actor == null) return NotFound();

            if (!string.IsNullOrEmpty(actor.ImagePath))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", actor.ImagePath);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            _context.actors.Remove(actor);
            _context.SaveChanges();

            TempData["Notification"] = "Actor deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}




