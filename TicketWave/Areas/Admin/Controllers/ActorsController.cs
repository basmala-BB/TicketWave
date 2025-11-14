using Magnum.FileSystem;
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
using Directory = System.IO.Directory;

namespace TicketWave.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class ActorsController : Controller
    {
        //private readonly ApplicationdbContext _context = new();
        private readonly IRepository<Actors> _ActorsRepository;

        public ActorsController(IRepository<Actors> actorsRepository)
        {
            _ActorsRepository = actorsRepository;
        }

        // ================= Index =================
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var actors = await _ActorsRepository.GetAsync(tracked: false , cancellationToken : cancellationToken);
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
        public async Task<IActionResult> Create(ActorVM actorVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(actorVM);
            }

            Actors actors = actorVM.Adapt<Actors>();

            if (actorVM.Path is not null && actorVM.Path.Length > 0)
            {

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(actorVM.Path.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    actorVM.Path.CopyTo(stream);
                }
                actors.ImagePath = fileName;
            }

            await _ActorsRepository.AddAsync(actors, cancellationToken);
            await _ActorsRepository.CommitAsync(cancellationToken);

            TempData["Notification"] = "Cinema created successfully!";
            return RedirectToAction(nameof(Index));

        }



        // ================= Edit GET =================
        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id , CancellationToken cancellationToken)
        {
            var actor = await _ActorsRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (actor == null) return NotFound();

            var vm = new ActorVM { Name = actor.Name };
            ViewBag.ActorId = actor.Id;
            ViewBag.ImagePath = actor.ImagePath;

            return View(vm);
        }

        // ================= Edit POST =================
        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id, ActorVM actorVM, CancellationToken cancellationToken)
        {
            var actors = await _ActorsRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (actors == null) return NotFound();

            if (!ModelState.IsValid)
                return View(actorVM);

            actors.Name = actorVM.Name;

            if (actorVM.Path != null && actorVM.Path.Length > 0)
            {
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(Path.GetFileName(actorVM.Path.FileName));
                var filePath = Path.Combine(imagesFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                actorVM.Path.CopyTo(stream);

                if (!string.IsNullOrEmpty(actors.ImagePath))
                {
                    var oldPath = Path.Combine(imagesFolder, actors.ImagePath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                actors.ImagePath = fileName;
            }
            _ActorsRepository.Update(actors);
            await _ActorsRepository.CommitAsync(cancellationToken);

            TempData["Notification"] = "Actor updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ================= Delete =================
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var actor = await _ActorsRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (actor == null) return NotFound();

            if (!string.IsNullOrEmpty(actor.ImagePath))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", actor.ImagePath);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            _ActorsRepository.Delete(actor);    
            await _ActorsRepository.CommitAsync(cancellationToken);

            TempData["Notification"] = "Actor deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}