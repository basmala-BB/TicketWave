using FilmPass.Data;
using FilmPass.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmPass.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly ApplicationdbContext _context = new();

        public IActionResult Index()
        {
            var cinemas = _context.cinemas.AsNoTracking().ToList();
            return View(cinemas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cinema cinema, IFormFile? ImageFile)
        {
            try
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                  
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    if (fileName.Length > 100)
                        fileName = fileName.Substring(0, 100);

                    var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(imagesFolder))
                        Directory.CreateDirectory(imagesFolder);

                    var filePath = Path.Combine(imagesFolder, fileName);

                   
                    try
                    {
                        using var stream = new FileStream(filePath, FileMode.Create);
                        ImageFile.CopyTo(stream);
                        cinema.ImagePath = fileName;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving image: {ex.Message}");
                        ModelState.AddModelError("ImageFile", "Error saving image. Please try again.");
                        return View(cinema); 
                    }
                }

                _context.cinemas.Add(cinema);
                _context.SaveChanges();

                TempData["Notification"] = "Cinema created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var fullError = ex.Message;
                if (ex.InnerException != null)
                    fullError += " | Inner Exception: " + ex.InnerException.Message;

                TempData["Error"] = fullError;
                return View(cinema);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cinema = _context.cinemas.FirstOrDefault(e => e.Id == id);
            if (cinema == null)
                return RedirectToAction("NotFoundPage", "Home");

            return View(cinema);
        }

        [HttpPost]
        public IActionResult Edit(Cinema cinema, IFormFile? img)
        {
            try
            {
                var cinemaInDb = _context.cinemas.AsNoTracking().FirstOrDefault(e => e.Id == cinema.Id);
                if (cinemaInDb == null)
                    return RedirectToAction("NotFoundPage", "Home");

                if (img != null && img.Length > 0)
                {
                    var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(imagesFolder))
                        Directory.CreateDirectory(imagesFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Path.GetFileName(img.FileName));
                    var filePath = Path.Combine(imagesFolder, fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        img.CopyTo(stream);
                    }

                    
                    if (!string.IsNullOrEmpty(cinemaInDb.ImagePath))
                    {
                        var oldPath = Path.Combine(imagesFolder, cinemaInDb.ImagePath);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    cinema.ImagePath = fileName;
                }
                else
                {
                    cinema.ImagePath = cinemaInDb.ImagePath;
                }

                _context.cinemas.Update(cinema);
                _context.SaveChanges();

                TempData["Notification"] = "Update Cinema Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(cinema);
            }
        }

        public IActionResult Delete(int id)
        {
            var cinema = _context.cinemas.FirstOrDefault(e => e.Id == id);
            if (cinema == null)
                return RedirectToAction("NotFoundPage", "Home");

            try
            {
               
                if (!string.IsNullOrEmpty(cinema.ImagePath))
                {
                    var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    var oldPath = Path.Combine(imagesFolder, cinema.ImagePath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                _context.cinemas.Remove(cinema);
                _context.SaveChanges();

                TempData["Notification"] = "Delete Cinema Successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
