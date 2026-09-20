using LMS.Application.Interface;
using LMS.Application.Service;
using LMS.Domain;
using LMS.Domain.Common;
using LMS.WEB.Models;
using LSM.Persistence.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LMS.WEB.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILeaveService _leaveService;
        private readonly ILeaveRepository _leaveRepository;
        public EmployeeController(IUserService userService,ILeaveService leaveService,ILeaveRepository leaveRepository)
        {
            _userService = userService;
            _leaveService = leaveService;
            _leaveRepository = leaveRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
           
            if (!Request.Cookies.TryGetValue("UserId", out string? userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            string userName = Request.Cookies["UserName"] ?? "Employee";
            string userEmail = Request.Cookies["UserEmail"] ?? "";


            var allLeaves = await _leaveRepository.GetLeavesByUserIdAsync(userId);
            var leaveHistory = allLeaves.Select(l => new EmployeeLeaveHistoryItemViewModel
            {
                Id = l.Id,
                FromDate = l.FromDate,
                ToDate = l.ToDate,
                Reason = l.Reason,
                Status = l.Status,
                AdminRemarks = l.AdminRemarks,
                ReviewedAt = l.ReviewedAt,
                CreatedOn = l.CreatedOn
            }).ToList();

            var model= new EmployeeDashboardViewModel
            {
                EmployeeName = userName,
                EmployeeEmail = userEmail,
                TotalLeavesCount = leaveHistory.Count,
                PendingLeavesCount = leaveHistory.Count(x => x.Status == "Pending"),
                ApprovedLeavesCount = leaveHistory.Count(x => x.Status == "Approved"),
                RejectedLeavesCount = leaveHistory.Count(x => x.Status == "Rejected"),
                MyLeaves = leaveHistory
            };

            return View(model);
        }
        public async Task<IActionResult> Index()
        {
            var userId = Request.Cookies["UserId"];
            if(userId== null)
            {
                return RedirectToAction("Login", "Account");
            }
            var role = Request.Cookies["UserRole"];
            var result = await _userService.GetAllEmployeeAsync();
            if (result == null)
            {
                return null;
            }
            var userViewModel = result.Select(s => new UserViewModel
            {
                Id = s.Id,
                FullName = s.FullName,
                Email = s.Email,
                Role = s.Role,
                IsActive=s.IsActive

            }).ToList();
            
            return View(userViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new UserViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userId = Request.Cookies["UserId"];

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var role = Request.Cookies["UserRole"];

            if (role != "Admin")
            {
                TempData["ErrorMessage"] = "You don't have permission to edit employees.";
                return RedirectToAction("Index");
            }
            
            var user = new User
            {
                Id = model.Id,
                FullName = model.FullName,
                Email = model.Email,
                Role = model.Role,
                PasswordHash = PasswordHelper.HashPassword(model.Password),
                IsActive = model.IsActive,
                CreatedBy= Convert.ToInt32(userId),
                CreatedOn=DateTime.UtcNow

            };

            await _userService.CreateUserAsync(user);

            TempData["SuccessMessage"] = "Employee Created successfully.";

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = Request.Cookies["UserId"];

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var role = Request.Cookies["UserRole"];

            if (role != "Admin")
            {
                TempData["ErrorMessage"] = "You don't have permission to edit employees.";
                return RedirectToAction("Index");
            }
            var user = await _userService.GetUserByIdAsync(id);
            var model = new UserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };
            if (user == null)
            {
                return NotFound();
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {

            var userId = Request.Cookies["UserId"];

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var role = Request.Cookies["UserRole"];

            if (role != "Admin")
            {
                TempData["ErrorMessage"] = "You don't have permission to edit employees.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.GetUserByIdAsync(model.Id);

            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Role = model.Role;
            user.IsActive = model.IsActive;

            // Only change password if user entered a new password
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.PasswordHash = PasswordHelper.HashPassword(model.Password);
            }

            user.UpdatedBy = int.Parse(userId);
            user.UpdatedON = DateTime.UtcNow;

            bool result=await _userService.UpdateUserAsync(user,Convert.ToInt32(userId));

            TempData["SuccessMessage"] = "Employee updated successfully.";

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            var result = await _userService.UpdateStatusAsync(id, isActive);

            if (!result)
            {
                return Json(new
                {
                    success = false,
                    message = "Unable to update employee status."
                });
            }

            return Json(new
            {
                success = true
            });
        }
    }
}
