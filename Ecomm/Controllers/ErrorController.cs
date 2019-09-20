using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/{statusCode:int}")]
        public IActionResult Index(int statusCode)
        {
            var evm = new Models.ErrorViewModel();
            ViewBag.StatusCode = statusCode;
            return View("Error", evm);
        }
    }
}