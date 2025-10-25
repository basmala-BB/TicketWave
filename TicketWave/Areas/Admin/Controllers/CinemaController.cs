using TicketWave.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketWave.ViewModel;
using Mapster;

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
            return View(new CreateCinemaVm());
        }
        [HttpPost]

        public IActionResult Create(CreateCinemaVm createCinemaVm)
        {
            if (!ModelState.IsValid)
            {
                return View(createCinemaVm);
            }

            //Cinema cinema = new()
            //{
            //    Name = createCinemaVm.Name,
            //    Status = createCinemaVm.Status,
            //    Description = createCinemaVm.Description,
            //};

            Cinema cinemas = createCinemaVm.Adapt<Cinema>();
            if (createCinemaVm.ImagePath is not null && createCinemaVm.ImagePath.Length > 0)
            {

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(createCinemaVm.ImagePath.FileName);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//images", fileName);


                using (var stream = System.IO.File.Create(filePath))
                {
                    createCinemaVm.ImagePath.CopyTo(stream);
                }

                cinemas.ImagePath = fileName;


            }

            _context.cinemas.Add(cinemas);
            _context.SaveChanges();

            TempData["Notification"] = "Cinema created successfully!";
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cinema = _context.cinemas.FirstOrDefault(e => e.Id == id);
            if (cinema == null)
                return RedirectToAction("NotFoundPage", "Home");

            return View(new updateCinemaVM()
            {
                Id = cinema.Id,
                Name = cinema.Name,
                Description = cinema.Description,
                Status = cinema.Status,
                ImagePath = cinema.ImagePath,
            });
        }

        [HttpPost]
        public IActionResult Edit(UpdateCinemaVM updateCinemaVM)
        {
            if (!ModelState.IsValid)
            {
                return View(updateCinemaVM);
            }

            var cinemaInDb = _context.cinemas.AsNoTracking().FirstOrDefault(e => e.Id == updateCinemaVM.Id);
            if (cinemaInDb == null)
                return RedirectToAction("NotFoundPage", "Home");
            //Cinema cinema = new()
            //{
            //    Name = updateCinemaVM.Name,
            //    Status = updateCinemaVM.Status,
            //    Description = updateCinemaVM.Description,

            //};
            Cinema cinemas = updateCinemaVM.Adapt<Cinema>();

            if (updateCinemaVM.NewImagePath is not null && updateCinemaVM.NewImagePath.Length > 0)
            {

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateCinemaVM.NewImagePath.FileName);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//images", fileName);


                using (var stream = System.IO.File.Create(filePath))
                {
                    updateCinemaVM.NewImagePath.CopyTo(stream);
                }

                var oldpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//images", cinemaInDb.ImagePath);
                if (System.IO.File.Exists(oldpath))
                {
                    System.IO.File.Delete(oldpath);
                }


                cinemas.ImagePath = fileName;
            }
            else
            {
                cinemas.ImagePath = cinemaInDb.ImagePath;
            }

            _context.cinemas.Update(cinemas);
            _context.SaveChanges();

            TempData["Notification"] = "Update Cinema Successfully";
            return RedirectToAction(nameof(Index));

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
