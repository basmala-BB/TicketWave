
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketWave.Models;
using TicketWave.Utitlies;

namespace YourProjectNamespace.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class DashboardController : Controller
    {
        ApplicationdbContext _context = new();

        public IActionResult Index()
        {
            var stats = new DashboardStats
            {
                TotalCategories = _context.categories.Count(),
                TotalActors = _context.actors.Count(),
                TotalCinemas = _context.cinemas.Count(),
                TotalMovies = _context.movies.Count()
            };

            return View(stats);
        }
    }
}

