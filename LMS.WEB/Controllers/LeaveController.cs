using LMS.Application.Interface;
using LMS.Domain;
using LMS.WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace LMS.WEB.Controllers
{
    public class LeaveController : Controller
    {
        private readonly ILeaveService _leaveService;
        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }
        public async Task<IActionResult> Index(LeaveViewModel model)
        {
            var userIdCookie = Request.Cookies["UserId"];

            if (!int.TryParse(userIdCookie, out var userId) || userId <= 0)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.EmployeeLeave = await _leaveService.GetLeavesByUserIdAsync(userId);

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ApplyLeave()
        {
            var userId = Request.Cookies["UserId"];

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var model = new LeaveViewModel(); 
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ApplyLeave(LeaveViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = int.Parse(Request.Cookies["UserId"]!);

            var request = new LeaveRequest
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                Reason = model.Reason
            };

            var result = await _leaveService.ApplyLeaveAsync(userId, request);

            if (!result.Success)
            {
                TempData["ErrorMessage"]= result.Message;
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;

            return RedirectToAction("Index");
        }
    }
}
