using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CodingLanguages.Controllers
{
    public class JavaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult example()
        {
            return View();
        }
    }
}
