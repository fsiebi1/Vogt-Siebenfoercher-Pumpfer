using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingLanguages.Controllers {
    public class UserController : Controller {
        public IActionResult Index() {
            return View();
        }
        public IActionResult Register() {
            return View();
        }
        public IActionResult Login() {
            return View();
        }
        public IActionResult Newsletter() {
            return View();
        }
    }
}
