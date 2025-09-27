using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskCoreCategoryProductsListPage.Models;

namespace TaskCoreCategoryProductsListPage.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ProductDbContext _dbContext;
        public AccountController(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();  
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserModel user)
        {
            if (!String.IsNullOrWhiteSpace(user.Email) && !String.IsNullOrWhiteSpace(user.Password))
            {
                if(user.Password == user.ConfirmPassword)
                {
                    user.Role = "User";
                    _dbContext.Users.Add(user);
                    _dbContext.SaveChanges();
                    return RedirectToAction("Login");
                }
              
            }
            ModelState.AddModelError("", "Registration Failed");
            return View(user);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserModel user, string? ReturnUrl)
        {
            if (user.Email != null && user.Password != null)
            {
                UserModel? user1 = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                if(user1 != null)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, user1.Email),
                        new Claim(ClaimTypes.Role, user1.Role),
                        new Claim(ClaimTypes.NameIdentifier, user1.Id.ToString()),
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, "MyAppCookieAuth");
                    HttpContext.SignInAsync("MyAppCookieAuth", new ClaimsPrincipal(claimsIdentity));
                    var userId = user1?.Id.ToString();
                    Response.Cookies.Append("userId", userId, new CookieOptions
                    {
                        HttpOnly = true,
                        //Expires = DateTimeOffset.UtcNow.AddDays(1) // optional
                    });
                    return RedirectToLocal(ReturnUrl);
                }
            }
            ModelState.AddModelError("", "Login Failed");
            return View(user);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        public IActionResult LogOff()
        {
            HttpContext.SignOutAsync("MyAppCookieAuth");
            return RedirectToAction("Login", "Account");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
