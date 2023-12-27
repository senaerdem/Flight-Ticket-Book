using FlightTicket.Business.Abstract;
using FlightTicket.Core;
using FlightTicket.Web.Models;
using FlightTicket.Web.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FlightTicket.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITripService _tripService;
        private readonly IMidlineService _midlineService;
        private LanguageService _localization;

        public HomeController(ITripService tripService, IMidlineService midlineService, LanguageService localization)
        {
            _tripService = tripService;
            _midlineService = midlineService;
            _localization = localization;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Welcome = _localization.Getkey("Welcome").Value;
            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;

            var midlines = await _midlineService.GetAllAsync();
            List<string> startingPoints = new List<string>();
            List<string> destinations = new List<string>();
            foreach (var midline in midlines)
            {
                if (!startingPoints.Contains(midline.StartingPoint))
                {
                    startingPoints.Add(midline.StartingPoint);
                }
                if (!destinations.Contains(midline.Destination))
                {
                    destinations.Add(midline.Destination);
                }
            };
            var tripSearchModel = new TripSearchModel()
            {
                StartingPoints = startingPoints,
                Destinations = destinations
            };
            return View(tripSearchModel);
        }

        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)), new CookieOptions()
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Index(TripSearchModel SearchModel)
        {
            if (User.IsInRole("Operator") || User.IsInRole("Admin"))
            {
                return View("_NoAccess");
            }
            if (ModelState.IsValid)
            {
                SearchModel.Date = Jobs.UpdateDateFormat(SearchModel.Date);
                return RedirectToAction("TripList", "FlightTicket", SearchModel);
            }
            else
            {
                var midlines = await _midlineService.GetAllAsync();
                List<string> startingPoints = new List<string>();
                List<string> destinations = new List<string>();
                foreach (var midline in midlines)
                {
                    if (!startingPoints.Contains(midline.StartingPoint))
                    {
                        startingPoints.Add(midline.StartingPoint);
                    }
                    if (!destinations.Contains(midline.Destination))
                    {
                        destinations.Add(midline.Destination);
                    }
                };

                SearchModel.StartingPoints = startingPoints;
                SearchModel.Destinations = destinations;

            }
            return View(SearchModel);
        }

    }
}