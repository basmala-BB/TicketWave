using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TicketWave.Models;
using TicketWave.ViewModel;


namespace TicketWave.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationdbContext _context;// = new();

        public HomeController(ILogger<HomeController> logger, ApplicationdbContext context)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<IActionResult> Item(int id, CancellationToken cancellationToken)
        {
            var Movie = await _context.movies
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (Movie is null)
                return NotFound();

            Movie.Traffic += 1;
            await _context.SaveChangesAsync(cancellationToken);

          
            var relatedMovies = await _context.movies
                .Where(e => e.CategoryId == Movie.CategoryId && e.Id != Movie.Id)
                .OrderByDescending(e => e.Traffic)
                .Take(4)
                .ToListAsync(cancellationToken);

            return View(new MovieWithRelatedVM
            {
                movie = Movie,
                RelatedMovies = relatedMovies
            });
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
