﻿using Microsoft.AspNetCore.Mvc;

namespace IdentityPractice.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
