
using TicketWave.Models;
using Microsoft.AspNetCore.Mvc;

namespace YourProjectNamespace.Areas.Admin.Controllers
{
    [Area("Admin")] 
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

