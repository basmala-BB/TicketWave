using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Linq.Expressions;
using TicketWave.Models;
using TicketWave.Repositories.IRepositories;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;

namespace TicketWave.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IMovieRepository _movieRepository;

        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository
        , IRepository<Promotion> promotionRepository, IMovieRepository productRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _promotionRepository = promotionRepository;
            _movieRepository = productRepository;
        }

        public async Task<IActionResult> Index(string code)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.movie, e => e.ApplicationUser]);

            var promotion = await _promotionRepository.GetOneAsync(e => e.Code == code && e.IsValid);

            if (promotion is not null)
            {
                var result = cart.FirstOrDefault(e => e.MovieId == promotion.moviId);

                if (result is not null)
                    result.Price -= result.movie.price * (promotion.Discount / 100);

                await _cartRepository.CommitAsync();
            }

            return View(cart);
        }
        public async Task<IActionResult> AddToCart(int count, int MovieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var movieInDb = await _cartRepository.GetOneAsync(
                e => e.ApplicationUserId == user.Id && e.MovieId == MovieId
            );

            if (movieInDb is not null)
            {
                movieInDb.Count += count;
                await _cartRepository.CommitAsync(cancellationToken);

                TempData["success-notification"] = "Update Product Count to cart successfully";
                // redirect to Cart page instead of Home
                return RedirectToAction("Index", "Cart");
            }

            await _cartRepository.AddAsync(new()
            {
                MovieId = MovieId,
                Count = count,
                ApplicationUserId = user.Id,
                Price = (await _movieRepository.GetOneAsync(e => e.Id == MovieId)!).price
            }, cancellationToken);

            await _cartRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Add Product to cart successfully";
            // redirect to Cart page instead of Home
            return RedirectToAction("Index", "Cart");
        }


        public async Task<IActionResult> IncrementMovie(int MovieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var product = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == MovieId);

            if(product is null) return NotFound();

            product.Count += 1;
            await _cartRepository.CommitAsync(cancellationToken);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DecrementMovie(int MovieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var product = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == MovieId);

            if (product is null) return NotFound();

            if(MovieId <= 1)
                _cartRepository.Delete(product);
            else
                product.Count -= 1;

            await _cartRepository.CommitAsync(cancellationToken);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteMovie(int MovieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var product = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == MovieId);

            if (product is null) return NotFound();

            _cartRepository.Delete(product);
            await _cartRepository.CommitAsync(cancellationToken);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.movie]);

            if (cart is null) return NotFound();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/cancel",
            };

            foreach (var item in cart)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.movie.Name,
                            Description = item.movie.Description,
                        },
                        UnitAmount = (long)item.Price * 100,
                    },
                    Quantity = item.Count,
                });
            }

            var service = new Stripe.Checkout.SessionService();

            var session = service.Create(options);
            return Redirect(session.Url);
        }

    }
}
