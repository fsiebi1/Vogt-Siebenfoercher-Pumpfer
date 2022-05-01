using CodingLanguages.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CodingLanguages.Controllers {     
    public class HomeController : Controller {

        public IActionResult Index() {

            return View(LoadLanguages());
        }
        private List<Languages> LoadLanguages()
        {
            return new List<Languages>()
            {
                new Languages() { languageName = "Java", info = "Ja", adv = "plattormunabhängig, starker Community-Support", disadv = "erheblich unperformanter als andere Programmiersprachen"},
                new Languages() { languageName = "NET", info = "Ja", adv = "plattformübergreifend, viele Bibliotheken bereitgestellt", disadv = "Lücke zwischen Release und Stabilität"},
                new Languages() { languageName = "Python", info = "Ja", adv = "benutzerfreundlich, unkompliziert und schnell", disadv = "Designeinschränkungen"}
            };
        }
        public IActionResult Impressum() {
            return View();
        }
        public IActionResult AboutUs() {
            return View();
        }
    }
}
