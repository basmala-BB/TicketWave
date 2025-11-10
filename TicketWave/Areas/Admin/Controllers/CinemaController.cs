using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TicketWave.Models;
using TicketWave.Repositories;
using TicketWave.Repositories.IRepositories;
using TicketWave.Utitlies;
using TicketWave.ViewModel;

namespace FilmPass.Areas.Admin.Controllers
{
    [Area("Admin")]

    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class CinemaController : Controller
    {
        private readonly IRepository<Cinema> _cinemaRepository;

        public CinemaController(IRepository<Cinema> cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var cinemas = await _cinemaRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            return View(cinemas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCinemaVm());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCinemaVm createCinemaVm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(createCinemaVm);
            }

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

            await _cinemaRepository.AddAsync(cinemas, cancellationToken);
            await _cinemaRepository.CommitAsync(cancellationToken);

            TempData["Notification"] = "Cinema created successfully!";
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var cinema = await _cinemaRepository.GetOneAsync(e => e.Id == id, tracked: false);
            if (cinema == null)
                return RedirectToAction("NotFoundPage", "Home");
            return View(cinema.Adapt<UpdateCinemaVM>());
;
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(UpdateCinemaVM updateCinemaVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(updateCinemaVM);
            }

            var cinemaInDb = await _cinemaRepository.GetOneAsync(e => e.Id == updateCinemaVM.Id, tracked: false);
            if (cinemaInDb == null)
                return RedirectToAction("NotFoundPage", "Home");
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
            _cinemaRepository.Update(cinemas);
            await _cinemaRepository.CommitAsync(cancellationToken);
            TempData["Notification"] = "Update Cinema Successfully";
            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var cinema = await _cinemaRepository.GetOneAsync(e => e.Id == id);
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
                _cinemaRepository.Delete(cinema);
                await _cinemaRepository.CommitAsync(cancellationToken);

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
