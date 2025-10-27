using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TicketWave.Models;
using TicketWave.Repositories;
using TicketWave.Repositories.IRepositories;

namespace TicketWave.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {

        private readonly ApplicationdbContext _context;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IRepository<Actors> _ActorsRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IRepository<MovieSubImages> _movieSubImagesRepository;

        public MovieController(
            ApplicationdbContext context,
            IRepository<Category> categoryRepository,
            IRepository<Cinema> cinemaRepository,
            IRepository<Actors> actorsRepository,
            IMovieRepository movieRepository,
            IRepository<MovieSubImages> movieSubImagesRepository)
        {
            _context = context;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _ActorsRepository = actorsRepository;
            _movieRepository = movieRepository;
            _movieSubImagesRepository = movieSubImagesRepository;
        }
        public async Task<IActionResult> Index()
        {
            var movies = await _movieRepository.GetAsync(tracked: false);
            return View(movies.AsEnumerable());
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAsync();
            var cinemas = await _cinemaRepository.GetAsync();
            var actors = await _ActorsRepository.GetAsync();
            return View(new MovieVM
            {
                categories = categories,
                cinemas = (IEnumerable<Movie>)cinemas,
                actors = actors,
            });
        }

        [HttpPost]
    
        public async Task<IActionResult> Create(MovieVM model, IFormFile Img, List<IFormFile>? SubImgs , CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.categories = await _context.categories.ToListAsync(cancellationToken);
                    model.cinemas = await _context.cinemas.ToListAsync(cancellationToken);
                    model.actors = await _context.actors.ToListAsync(cancellationToken);
                    return View(model);
                }

               
                var mainFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(mainFolder))
                    Directory.CreateDirectory(mainFolder);

                var movie = model.Movie;

                if (Img is not null && Img.Length > 0)
                {
                    // Save Img in wwwroot
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName); // 30291jsfd4-210klsdf32-4vsfksgs.png
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        Img.CopyTo(stream);
                    }

                    // Save Img in db
                    movie.MainImg = fileName;
                }

                if (model.ActorsIds != null && model.ActorsIds.Any())
                {
                    foreach (var actorId in model.ActorsIds)
                    {
                        var actor = _context.actors.Find(actorId);
                        if (actor != null)
                            movie.Actors.Add(actor);
                    }
                }

               await _movieRepository.AddAsync(movie, cancellationToken);
                await _movieRepository.CommitAsync(cancellationToken);

       
                if (SubImgs != null && SubImgs.Count > 0)
                {
                    var subFolder = Path.Combine(mainFolder, "Movie_images");
                    if (!Directory.Exists(subFolder))
                        Directory.CreateDirectory(subFolder);

                    foreach (var item in SubImgs)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName); // 30291jsfd4-210klsdf32-4vsfksgs.png
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            item.CopyTo(stream);
                        }

                        await _movieSubImagesRepository.AddAsync(new MovieSubImages
                        {
                            ImagePath = fileName,
                            Id = movie.Id,
                        });
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                }

                TempData["Notification"] = "Movie created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                model.categories = _context.categories.ToList();
                model.cinemas = _context.cinemas.ToList();
                model.actors = _context.actors.ToList();
                return View(model);
            }

        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken cancellationToken)
        {
            var Movie = await _movieRepository.GetOneAsync();

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
        public async Task<IActionResult> Edit(Movie Movie, IFormFile? img , CancellationToken cancellationToken)
        {
            var MovieInDb = await _movieRepository.GetOneAsync((System.Linq.Expressions.Expression<Func<Movie, bool>>?)(e => e.Id == Movie.Id) , tracked : false);
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

             _movieRepository.Update(Movie);
            _movieRepository.Delete(Movie);

            TempData["Notification"] = "Update Movie Successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id , CancellationToken cancellationToken)
        {
            var Movie = await _movieRepository.GetOneAsync(e => e.Id == id);

            if (Movie is null)
                return RedirectToAction("NotFoundPage", "Home");

            
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", (string)Movie.MainImg);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }

            _movieRepository.Delete((Movie)Movie);
           await _movieRepository.CommitAsync(cancellationToken);

            TempData["Notification"] = "Delete Movie Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
