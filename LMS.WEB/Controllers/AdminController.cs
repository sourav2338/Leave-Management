using LMS.Application.Interface;
using LMS.WEB.Models;
using LSM.Persistence.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LMS.WEB.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly ILeaveService _leaveService;
        public AdminController(IUserRepository userRepository,ILeaveRepository leaveRepository,ILeaveService leaveService)
        {
            _userRepository = userRepository;
            _leaveRepository = leaveRepository;
            _leaveService = leaveService;
        }
        public async Task<IActionResult> Dashboard()
        {
            var userId = Request.Cookies["UserId"];
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var role = Request.Cookies["UserRole"];
            if (role != "Admin")
                return null;

            var userName = Request.Cookies["UserName"] ?? "User";

            var hour = DateTime.Now.Hour;

            var greeting = hour switch
            {
                >= 5 and < 12 => "Good Morning",
                >= 12 and < 17 => "Good Afternoon",
                >= 17 and < 21 => "Good Evening",
                _ => "Good Night"
            };
            var model = new AdminDashboardViewModel();
            ViewBag.Message = $"{greeting}, {userName} 👋";

            model.TotalEmployees = await _userRepository.GetTotalEmployeesCountAsync();
            var(pending,approved,rejected) = await _leaveRepository.GetSummaryCountsAsync();
            model.TotalPendingRequests = pending;
            model.TotalApprovedRequests = approved;
            model.TotalRejectedRequests = rejected;
            var dynamics = await _leaveRepository.GetAllLeavesWithEmployeeDetailsAsync();
            List<AdminLeaveItemViewModel> lst = dynamics.Select(d => new AdminLeaveItemViewModel
            {
                Id = (int)d.Id,
                UserId = (int)d.UserId,
                EmployeeName = (string)d.EmployeeName,
                EmployeeEmail = (string)d.EmployeeEmail,
                FromDate = (DateTime)d.FromDate,
                ToDate = (DateTime)d.ToDate,
                Reason = (string)d.Reason,
                Status = (string)d.Status,
                AdminRemarks = (string?)d.AdminRemarks,
                ReviewedAt = (DateTime?)d.ReviewedAt,
                CreatedOn = (DateTime)d.CreatedOn
            }).ToList();

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ManageLeaves(string? statusFilter)
        {           
            var dynamics = await _leaveService.GetAllLeaveRequestsAsync();
            List<AdminLeaveItemViewModel> lst = dynamics.Select(d => new AdminLeaveItemViewModel
            {
                Id = (int)d.Id,
                UserId = (int)d.UserId,
                EmployeeName = (string)d.EmployeeName,
                EmployeeEmail = (string)d.EmployeeEmail,
                FromDate = (DateTime)d.FromDate,
                ToDate = (DateTime)d.ToDate,
                Reason = (string)d.Reason,
                Status = (string)d.Status,
                AdminRemarks = (string?)d.AdminRemarks,
                ReviewedAt = (DateTime?)d.ReviewedAt,
                CreatedOn = (DateTime)d.CreatedOn
            }).ToList();

            if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "All")
            {
                lst = lst.Where(l => string.Equals(l.Status, statusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.CurrentStatusFilter = statusFilter ?? "All";
            return View(lst);
        }

      
        [HttpPost]
        public async Task<IActionResult> UpdateLeaveStatus(int id, string status, string? remarks)
        {
            
            if (!Request.Cookies.TryGetValue("UserId", out string? adminIdStr) || !int.TryParse(adminIdStr, out int adminId))
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Validate requested status[cite: 1]
            if (status != "Approved" && status != "Rejected")
            {
                TempData["Error"] = "Invalid action status requested.";
                return RedirectToAction(nameof(ManageLeaves));
            }

          
            var result = await _leaveService.ReviewLeaveRequestAsync(id, status, remarks, adminId);

            if (result)
            {
                TempData["Success"] = "Leave Approved";
            }

            return RedirectToAction(nameof(ManageLeaves));
        }
    }
}
