using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Helper;
using ArtStation.Core.Services.Contract;
using ArtStation.Core.Statistics;
using ArtStation_Dashboard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ArtStation_Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStatisticsService _statisticsService;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(ILogger<HomeController> logger
            ,IStatisticsService statisticsService
            , UserManager<AppUser> userManager)
        {
            _logger = logger;
           _statisticsService = statisticsService;
           _userManager = userManager;
        }
        public IActionResult SetLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            StatisticsDTO statistics = new StatisticsDTO();
            if (User.IsInRole("Admin"))
            {
                 statistics = await _statisticsService.GetAdminStatistics();

            }
            if (User.IsInRole("Trader"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(userId);
                statistics = _statisticsService.GetCompanyStatistics(user.PhoneNumber);
            }
            return View(statistics);
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
