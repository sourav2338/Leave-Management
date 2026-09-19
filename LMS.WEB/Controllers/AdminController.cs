using LSM.Persistence.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LMS.WEB.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;
        public AdminController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.Employee = await _userRepository.GetTotalEmployeesCountAsync();
            return View();
        }
    }
}
