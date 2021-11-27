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
            return View();
        }
        public IActionResult Impressum() {
            return View();
        }
        public IActionResult AboutUs() {
            return View();
        }
    }
}
