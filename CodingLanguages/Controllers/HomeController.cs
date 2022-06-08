using CodingLanguages.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

namespace CodingLanguages.Controllers {     
    public class HomeController : Controller {

        private readonly IStringLocalizer<HomeController> _stringLocalizer;
        public HomeController(IStringLocalizer<HomeController> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        public IActionResult Index() {
            return View(LoadLanguages());
        }

        private List<Languages> LoadLanguages()
        {
            return new List<Languages>()
            {
                new Languages() { languageName = "Java", info = _stringLocalizer["index.yes"].Value, adv = _stringLocalizer["index.java.adv"].Value, disadv = _stringLocalizer["index.java.disadv"].Value},
                new Languages() { languageName = "NET", info = _stringLocalizer["index.yes"].Value, adv = _stringLocalizer["index.net.adv"].Value, disadv = _stringLocalizer["index.net.disadv"].Value},
                new Languages() { languageName = "Python", info = _stringLocalizer["index.yes"].Value, adv = _stringLocalizer["index.py.adv"].Value, disadv = _stringLocalizer["index.py.disadv"].Value}
            };
        }
        public IActionResult Impressum() {
            return View();
        }

        [HttpPost]
        public IActionResult ChangeLanguage(string returnUrl)
        {
            string culture; 

            if (HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture.Name == "de-DE")
            {
                culture = "en-US";
            } else
            {
                culture = "de-DE";
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    }
            );

            return LocalRedirect(returnUrl);
        }

        public IActionResult AboutUs() {
            return View();
        }
    }
}
