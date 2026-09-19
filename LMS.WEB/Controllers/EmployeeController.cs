using LMS.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LMS.WEB.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IUserService _userService;
        public EmployeeController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
           var result=await _userService.GetAllEmployeeAsync();
           return View();
        }
    }
}
