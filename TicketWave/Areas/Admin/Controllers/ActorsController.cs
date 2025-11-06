using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TicketWave.Models;
using TicketWave.Repositories;
using TicketWave.Repositories.IRepositories;
using TicketWave.Utitlies;

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
        public async Task<IActionResult> Create(ActorVM model , CancellationToken cancellationToken)
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

            await _ActorsRepository.AddAsync(new Actors
            {
                Name = model.Name,
                ImagePath = fileName
            }, cancellationToken);
           await _ActorsRepository.CommitAsync(cancellationToken);

            TempData["Notification"] = "Actor added successfully!";
            return RedirectToAction("Index");
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
        public async Task<IActionResult> Edit(int id, ActorVM model , CancellationToken cancellationToken)
        {
            var actor = await _ActorsRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
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
            _ActorsRepository.Update(actor);
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