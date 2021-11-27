using CodingLanguages.Models;
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
                new Languages() { languageName = "Java", info = "is halt java", adv = "x", disadv = "y"},
                new Languages() { languageName = "NET", info = "besser als java", adv = "x", disadv = "y"}
            };
        }
    }
}
