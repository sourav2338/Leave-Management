
using Microsoft.AspNetCore.Mvc;
using LMS.Application.Interface;

using LMS.WEB.Models;

namespace LMS.WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }           
              

            var user = await _userService.ValidateUserCredentialsAsync(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email/password or user is inactive.");
                return View(model);
            }

           
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,                                     
                Expires = DateTimeOffset.UtcNow.AddHours(8),         
                IsEssential = true                                  
            };

          
            Response.Cookies.Append("UserId", user.Id.ToString(), cookieOptions);
            Response.Cookies.Append("UserRole", user.Role, cookieOptions);
            Response.Cookies.Append("UserName", user.FullName, cookieOptions);

            return user.Role == "Admin"
                ? RedirectToAction("Dashboard", "Admin")
                : RedirectToAction("Dashboard", "Employee");
        }
    }
}
