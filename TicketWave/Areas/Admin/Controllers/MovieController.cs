using FilmPass.Data;
using FilmPass.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmPass.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        ApplicationdbContext _context = new();
        public IActionResult Index()
        {
            var movies = _context.movies.AsNoTracking().AsQueryable();


            return View(movies.AsEnumerable());
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.categories;
            var cinemas = _context.cinemas;
            var actors = _context.actors; 

            return View(new MovieVM
            {
                categories = categories.AsEnumerable(),
                cinemas = cinemas.AsEnumerable(),
                actors = actors.AsEnumerable(),
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MovieVM model, IFormFile img, List<IFormFile>? subImgs)
        {
            if (!ModelState.IsValid)
            {
                model.categories = _context.categories.ToList();
                model.cinemas = _context.cinemas.ToList();
                model.actors = _context.actors.ToList();
                return View(model);
            }

            // رفع الصورة الرئيسية
            if (img is not null && img.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);
                }

                model.Movie.MainImg = fileName;
            }

            // إضافة Movie جديد
            var movie = model.Movie;

            // إضافة Actors مختارين
            if (model.ActorsIds != null)
            {
                foreach (var actorId in model.ActorsIds)
                {
                    var actor = _context.actors.Find(actorId);
                    if (actor != null)
                        movie.Actors.Add(actor);
                }
            }

            _context.movies.Add(movie);
            _context.SaveChanges();

            if (subImgs != null && subImgs.Count > 0)
            {
                foreach (var item in subImgs)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Movie_images");
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    var filePath = Path.Combine(folder, fileName);


                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }

                    _context.movieSubImages.Add(new()
                    {
                        ImagePath = fileName,
                        MovieId = movie.Id,
                    });
                }

                _context.SaveChanges();
            }

            TempData["Notification"] = "Add Movie Successfully";
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public IActionResult Edit(int id)
        {
            var Movie = _context.movies.FirstOrDefault(e => e.Id == id);

            if (Movie is null)
                return RedirectToAction("NotFoundPage", "Home");

       
            var categories = _context.categories;
      
            var cinemas = _context.cinemas;

            return View(new MovieVM
            {
                categories = categories.AsEnumerable(),
                cinemas = cinemas.AsEnumerable(),
                Movie = Movie,
            });
        }

        [HttpPost]
        public IActionResult Edit(Movie Movie, IFormFile? img)
        {
            var MovieInDb = _context.movies.AsNoTracking().FirstOrDefault(e => e.Id == Movie.Id);
            if (MovieInDb is null)
                return RedirectToAction("NotFoundPage", "Home");

            if (img is not null)
            {
                if (img.Length > 0)
                {
                  
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName); 
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        img.CopyTo(stream);
                    }

                  
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", MovieInDb.MainImg);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                   
                    Movie.MainImg = fileName;
                }
            }
            else
            {
                Movie.MainImg = MovieInDb.MainImg;
            }

            _context.movies.Update(Movie);
            _context.SaveChanges();

            TempData["Notification"] = "Update Movie Successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var Movie = _context.movies.FirstOrDefault(e => e.Id == id);

            if (Movie is null)
                return RedirectToAction("NotFoundPage", "Home");

            
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", Movie.MainImg);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }

            _context.movies.Remove(Movie);
            _context.SaveChanges();

            TempData["Notification"] = "Delete Movie Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
