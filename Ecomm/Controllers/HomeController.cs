using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ecomm.Models;
using Ecomm.DataAccess;
using EComm.Model;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Controllers
{
    public class HomeController : Controller
    {
        private ECommDataContext _dataContext;

        public HomeController(ECommDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            return View();
            //return Content("Hello from HomeController");
            //return Content("<em>Hello</em> from homeController", "text/html");
            //return Content("{\"Greeting\":\"Hello\"}", "application/json");
            //return Content($"Number of products: {_dataContext.Products.Count()}");

            //var products = _dataContext.Products.Include(p => p.Supplier).ToList();
            //return View(products);
            //return View("Missing");
        }


        //public List<Product> Index()
        //{
        //    return _dataContext.Products.ToList();
        //}

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
