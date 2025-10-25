using System.Diagnostics;
using TicketWave.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilmPass.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller

    {
        public IActionResult NotFoundPage()
        {
            return View();
        }
    }
}
