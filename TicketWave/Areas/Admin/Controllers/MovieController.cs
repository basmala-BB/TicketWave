using TicketWave.Models;
using TicketWave.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using TicketWave.Repositories.IRepositories;
using TicketWave.Utitlies;
using TicketWave.ViewModel;

namespace TicketWave.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class MovieController : Controller
    {
        private readonly ApplicationdbContext _context;
        private readonly IMovieRepository _movieRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IRepository<Actors> _actorRepository;
        private readonly IRepository<MovieSubImage> _movieSubImageRepository;
        private readonly IRepository<MovieActor> _movieActorRepository;

        public MovieController(
             ApplicationdbContext context,
            IMovieRepository movieRepository,
            IRepository<Category> categoryRepository,
            IRepository<Cinema> cinemaRepository,
            IRepository<Actors> actorRepository,
            IRepository<MovieSubImage> movieSubImageRepository,
            IRepository<MovieActor> movieActorRepository)
        {
            _context = context;
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _movieSubImageRepository = movieSubImageRepository;
            _movieActorRepository = movieActorRepository;
        }

        public async Task<IActionResult> Index(FilterMovieVM filterMovieVM, CancellationToken cancellationToken, int page = 1)
        {
            var movies = await _movieRepository.GetAsync(includes: [e => e.Category, e => e.Cinema], tracked: false, cancellationToken: cancellationToken);

            #region Filter Movie
            if (!string.IsNullOrEmpty(filterMovieVM.Title))
            {
                movies = movies.Where(e => e.Name.Contains(filterMovieVM.Title.Trim()));
                ViewBag.Title = filterMovieVM.Title;
            }

            if (filterMovieVM.CategoryId != null)
            {
                movies = movies.Where(e => e.CategoryId == filterMovieVM.CategoryId);
                ViewBag.CategoryId = filterMovieVM.CategoryId;
            }

            if (filterMovieVM.CinemaId != null)
            {
                movies = movies.Where(e => e.CinemaId == filterMovieVM.CinemaId);
                ViewBag.CinemaId = filterMovieVM.CinemaId;
            }
            //if (filterMovieVM.ActorId != null)
            //{
            //    movies = movies.Where(e => e.MovieActors == filterMovieVM.ActorId);
            //    ViewBag.ActorId = filterMovieVM.ActorId;
            //}

            var categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Categories = categories.AsEnumerable();

            var cinemas = await _cinemaRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Cinemas = cinemas.AsEnumerable();
            #endregion

            #region Pagination
            ViewBag.TotalPages = Math.Ceiling(movies.Count() / 8.0);
            ViewBag.CurrentPage = page;
            movies = movies.Skip((page - 1) * 8).Take(8);
            #endregion

            return View(movies.AsEnumerable());
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAsync(cancellationToken : cancellationToken) ?? new List<Category>();
            var cinemas = await _cinemaRepository.GetAsync(cancellationToken: cancellationToken) ?? new List<Cinema>();
            var actors = await _actorRepository.GetAsync(cancellationToken: cancellationToken) ?? new List<Actors>();

            var model = new MovieVM
            {
                Categories = categories,
                cinemas = cinemas,
                actors = actors,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile Img, List<IFormFile>? subImgs, int[] actorIds, CancellationToken cancellationToken)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (Img != null && Img.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        Img.CopyTo(stream);
                    }

                    movie.MainImg = fileName;
                }

                var movieCreated = await _movieRepository.AddAsync(movie, cancellationToken);
                await _movieRepository.CommitAsync(cancellationToken);

                if (subImgs != null && subImgs.Count > 0)
                {
                    foreach (var item in subImgs)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            item.CopyTo(stream);
                        }

                        await _movieSubImageRepository.AddAsync(new()
                        {
                            ImagePath = fileName,
                            MovieId = movieCreated.Id,
                        }, cancellationToken);
                    }
                    await _movieSubImageRepository.CommitAsync(cancellationToken);
                }

                if (actorIds.Any())
                {
                    foreach (var actorId in actorIds)
                    {
                        await _movieActorRepository.AddAsync(new()
                        {
                            ActorId = actorId,
                            MovieId = movieCreated.Id,
                        }, cancellationToken);
                    }
                    await _movieActorRepository.CommitAsync(cancellationToken);
                }

                TempData["success-notification"] = "Add Movie Successfully";
                transaction.Commit();
            }
            catch
            {
                TempData["error-notification"] = "Error While Saving the movie";
                transaction.Rollback();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id, includes: [e => e.movieSubImages, e => e.MovieActor], tracked: false, cancellationToken: cancellationToken);
            if (movie == null)
                return RedirectToAction("NotFoundPage", "Home");

            var categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
            var cinemas = await _cinemaRepository.GetAsync(cancellationToken: cancellationToken);

            return View(new MovieVM
            {
                Categories = categories.AsEnumerable(),
                cinemas = cinemas.AsEnumerable(),
                movie = movie,
            });
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(Movie movie, IFormFile? Img, List<IFormFile>? subImgs, int[] actorIds, CancellationToken cancellationToken)
        {
            var movieInDb = await _movieRepository.GetOneAsync(e => e.Id == movie.Id, includes: [e => e.MovieActor], tracked: false, cancellationToken: cancellationToken);
            if (movieInDb == null)
                return RedirectToAction("NotFoundPage", "Home");

            if (Img != null && Img.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    Img.CopyTo(stream);
                }

                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", movieInDb.MainImg);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);

                movie.MainImg = fileName;
            }
            else
            {
                movie.MainImg = movieInDb.MainImg;
            }

            _movieRepository.Update(movie);
            await _movieRepository.CommitAsync(cancellationToken);

            if (subImgs != null && subImgs.Count > 0)
            {
                movie.movieSubImages = new List<MovieSubImage>();
                foreach (var item in subImgs)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }

                    movie.movieSubImages.Add(new()
                    {
                        ImagePath = fileName,
                        MovieId = movie.Id,
                    });
                }
                await _movieSubImageRepository.CommitAsync(cancellationToken);
            }

            TempData["success-notification"] = "Update Movie Successfully";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id, includes: [e => e.movieSubImages, e => e.MovieActor], tracked: false, cancellationToken: cancellationToken);
            if (movie == null)
                return RedirectToAction("NotFoundPage", "Home");

            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", movie.MainImg);
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);

            foreach (var subImg in movie.movieSubImages)
            {
                var subImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images", subImg.ImagePath);
                if (System.IO.File.Exists(subImgPath))
                    System.IO.File.Delete(subImgPath);
            }

            _movieRepository.Delete(movie);
            await _movieRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Delete Movie Successfully";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> DeleteSubImg(int movieId, string Img, CancellationToken cancellationToken)
        {
            var movieSubImg = await _movieSubImageRepository.GetOneAsync(e => e.MovieId == movieId && e.ImagePath == Img);
            if (movieSubImg == null)
                return RedirectToAction("NotFoundPage", "Home");

            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images", movieSubImg.ImagePath);
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);

            _movieSubImageRepository.Delete(movieSubImg);
            await _movieSubImageRepository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Edit), new { id = movieId });
        }
    }
}
