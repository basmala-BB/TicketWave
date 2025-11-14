
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using TicketWave.Models;
using TicketWave.Repositories.IRepositories;
using TicketWave.Utitlies;

namespace TicketWave.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class PromotionController : Controller
    {
        ApplicationdbContext _context = new();
        private readonly IRepository<Promotion> _promotionRepository;// = new();
        private readonly IMovieRepository _movieRepository;

        public PromotionController(IRepository<Promotion> promotionRepository, IMovieRepository movieRepository)
        {
            _promotionRepository = promotionRepository;
            _movieRepository = movieRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var promotions = await _promotionRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);

            // Add Filter

            return View(promotions);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.movie = await _movieRepository.GetAsync(tracked: false);

            return View(new Promotion());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Promotion promotion, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.movie = await _movieRepository.GetAsync(tracked: false);

                return View(promotion);
            }

            // Save brand in db
            await _promotionRepository.AddAsync(promotion, cancellationToken);
            await _promotionRepository.CommitAsync(cancellationToken);

            //Response.Cookies.Append("success-notification", "Add Brand Successfully");
            TempData["success-notification"] = "Add Promotion Successfully";

            //return View(nameof(Index));
            return RedirectToAction(nameof(Create));
        }
    }
}
