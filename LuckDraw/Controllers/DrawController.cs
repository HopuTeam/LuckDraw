﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Controllers
{
    public class DrawController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}