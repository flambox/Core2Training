﻿using Microsoft.AspNetCore.Mvc;
using Ecomm.Models;
using Ecomm.DataAccess;
using EComm.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Ecomm.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Ecomm.Controllers
{
    public class ProductController : Controller
    {
        private ECommDataContext _dataContext;

        public ProductController(ECommDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Authorize(Policy = "AdminsOnly")]
        [Route("product/{id:int}")]
        public IActionResult Detail(int id)
        {
            var product = _dataContext.Products.Include(p => p.Supplier).SingleOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        public IActionResult _DetailPartial(int id)
        {
            var product = _dataContext.Products.Include(p => p.Supplier).SingleOrDefault(p => p.Id == id);
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _dataContext.Products.SingleOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();


            var suppliers = _dataContext.Suppliers.ToList();
            var pvm = new ProductEditViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                Package = product.Package,
                IsDiscontinued = product.IsDiscontinued,
                SupplierId = product.SupplierId,
                Suppliers = suppliers.Select(s => new SelectListItem { Text = s.CompanyName, Value = s.Id.ToString() }).ToList()
            };
            return View(pvm);
        }

        [HttpPost]
        public IActionResult Edit(ProductEditViewModel pvm)
        {
            if (!ModelState.IsValid)
            {
                var suppliers = _dataContext.Suppliers.ToList();
                pvm.Suppliers = suppliers.Select(s => new SelectListItem
                {
                    Text = s.CompanyName,
                    Value = s.Id.ToString()
                }).ToList();
                return View(pvm);
            }
            
            var product = new Product
            {
                Id = pvm.Id,
                ProductName = pvm.ProductName,
                UnitPrice = pvm.UnitPrice, 
                Package = pvm.Package,
                IsDiscontinued = pvm.IsDiscontinued,
                SupplierId = pvm.SupplierId,
                
            };
            _dataContext.Attach(product);
            _dataContext.Entry(product).State = EntityState.Modified;
            _dataContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult AddToCart(int id, int quantity)
        {
            var product = _dataContext.Products.SingleOrDefault(p => p.Id == id);
            var totalCost = product.UnitPrice * quantity;
            string message = $"You added {product.ProductName} " +
                $"(x {quantity}) to your cart " +
                $"at a total cost of {totalCost:C}.";

            var cart = ShoppingCart.GetFromSession(HttpContext.Session);
            var lineItem = cart.LineItems.SingleOrDefault(item => item.Product.Id == id);

            if (lineItem != null)
            {
                lineItem.Quantity += quantity;
            }
            else
            {
                cart.LineItems.Add(new ShoppingCart.LineItem{
                    Product = product,
                    Quantity = quantity
                });
            }
            ShoppingCart.StoreInSession(cart, HttpContext.Session);
            return PartialView("_AddedToCartPartial", message);
        }

        public IActionResult Cart()
        {
            var cart = ShoppingCart.GetFromSession(HttpContext.Session);
            var cvm = new CartViewModel() { Cart = cart };
            return View(cvm);
        }

        [HttpPost]
        public IActionResult Checkout(CartViewModel cvm)
        {
            if (!ModelState.IsValid)
            {
                cvm.Cart = ShoppingCart.GetFromSession(HttpContext.Session);
                return View("Cart", cvm);
            }
            // TODO: Charge the customer's card and record the order
            HttpContext.Session.Clear();
            return View("ThankYou");
        }

        [HttpGet ("api/products")]
        public IActionResult Get()
        {
            var products = _dataContext.Products.ToList();
            return new ObjectResult(products);
        }

        [HttpGet ("api/products/{id:int}")]
        public IActionResult Get( int id)
        {
            var product = _dataContext.Products.SingleOrDefault(p => p.Id == id);
            if (product != null)
            {
                return new ObjectResult(product);
            }
            return NotFound();
        }

        [HttpPut ("api/products/{id:int}")]
        public IActionResult Put(int id, [FromBody]Product product)
        {
            if (product == null || product.Id != id)
                return BadRequest();

            var existing = _dataContext.Products.SingleOrDefault(p => p.Id == id);
            if (existing == null)
                return NotFound();
            existing.ProductName = product.ProductName;
            existing.UnitPrice = product.UnitPrice;
            existing.Package = product.Package;
            existing.IsDiscontinued = product.IsDiscontinued;
            existing.SupplierId = product.SupplierId;
            _dataContext.SaveChanges();

            return new NoContentResult();
        }

        //[HttpPut("api/products/{id:int}")]
        //public IActionResult Put(int id, [FromBody]Product product)
        //{
        //    if (product == null || product.Id != id)
        //        return BadRequest();

        //    var existing = _dataContext.Products.SingleOrDefault(p => p.Id == id);
        //    if (existing == null)
        //        return NotFound();
        //    existing.ProductName = product.ProductName;
        //    existing.UnitPrice = product.UnitPrice;
        //    existing.Package = product.Package;
        //    existing.IsDiscontinued = product.IsDiscontinued;
        //    existing.SupplierId = product.SupplierId;
        //    _dataContext.SaveChanges();

        //    return new NoContentResult();
        //}
    }
}