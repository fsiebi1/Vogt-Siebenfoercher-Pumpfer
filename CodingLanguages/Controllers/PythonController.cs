﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingLanguages.Controllers
{
    public class PythonController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
