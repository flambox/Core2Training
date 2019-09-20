using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecomm.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login(string ReturnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = ReturnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            if (!ModelState.IsValid) return View(lvm);

            bool auth = (lvm.Username == "test" && lvm.Password == "password") ||
                        (lvm.Username == "test1" && lvm.Password == "pass");

            if (!auth) return View(lvm);

            ClaimsPrincipal principal = null;
            if (lvm.Username == "test")
            {
                principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
              {
                  new Claim(ClaimTypes.Name, lvm.Username),
                  new Claim(ClaimTypes.Role, "Admin")
              }, "FormsAuthentication"));
            }
            else if (lvm.Username == "test1")
            {
                principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
              {
                  new Claim(ClaimTypes.Name, lvm.Username),
                  new Claim(ClaimTypes.Role, "User")
              }, "FormsAuthentication"));
            }
            await HttpContext.SignInAsync(
              CookieAuthenticationDefaults.AuthenticationScheme,
              principal);

            if (lvm.ReturnUrl != null) return LocalRedirect(lvm.ReturnUrl);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            LoginViewModel lvm = new LoginViewModel();
            //lvm.ReturnUrl = "";
            return View("Login", lvm);
        }

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}