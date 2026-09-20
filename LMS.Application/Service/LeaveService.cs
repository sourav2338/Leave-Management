using LMS.Application.Interface;
using LMS.Domain;
using LMS.Domain.Common;
using LSM.Persistence.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Service
{
    public class LeaveService:ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IUserRepository _userRepository;

        public LeaveService(ILeaveRepository leaveRepository, IUserRepository userRepository)
        {
            _leaveRepository = leaveRepository;
            _userRepository = userRepository;
        }

       

        //public async Task<AdminDashboardViewModel> GetAdminDashboardAsync()
        //{
        //    var summary = await _leaveRepository.GetSummaryCountsAsync();
        //    var totalEmployees = await _userRepository.GetTotalEmployeesCountAsync();
        //    var allRawLeaves = await _leaveRepository.GetAllLeavesWithEmployeeDetailsAsync();

        //    var recentItems = allRawLeaves.Take(10).Select(l => new AdminLeaveItemViewModel
        //    {
        //        Id = l.Id,
        //        UserId = l.UserId,
        //        EmployeeName = l.EmployeeName,
        //        EmployeeEmail = l.EmployeeEmail,
        //        FromDate = l.FromDate,
        //        ToDate = l.ToDate,
        //        Reason = l.Reason,
        //        Status = l.Status,
        //        AdminRemarks = l.AdminRemarks,
        //        ReviewedAt = l.ReviewedAt,
        //        CreatedAt = l.CreatedAt
        //    }).ToList();

        //    return new AdminDashboardViewModel
        //    {
        //        TotalEmployees = totalEmployees,
        //        TotalPendingRequests = summary.Pending,
        //        TotalApprovedRequests = summary.Approved,
        //        TotalRejectedRequests = summary.Rejected,
        //        RecentRequests = recentItems
        //    };
        //}

        public async Task<ServiceResult> ApplyLeaveAsync(int userId, LeaveRequest model)
        {
            if (model.FromDate.Date < DateTime.Today)
            {
                return ServiceResult.Fail("Leave start date cannot be in the past.");
            }

            if (model.ToDate.Date < model.FromDate.Date)
            {
                return ServiceResult.Fail("End date must be greater than or equal to start date.");
            }
            bool hasOverlap = await _leaveRepository.HasOverlappingLeaveAsync(userId, model.FromDate.Date, model.ToDate.Date);
            if (hasOverlap)
            {
                return ServiceResult.Fail("You already have an active or pending leave during these dates.");
            }
            var request = new LeaveRequest
            {
                UserId = userId,
                FromDate = model.FromDate.Date,
                ToDate = model.ToDate.Date,
                Reason = model.Reason?.Trim(),
                Status = "Pending",
                CreatedOn = DateTime.UtcNow,
                CreatedBy=userId
            };
            await _leaveRepository.CreateLeaveRequestAsync(request);
            return ServiceResult.Ok("Leave application submitted successfully.");
        }

        //public async Task<ServiceResult> ReviewLeaveRequestAsync(int leaveId, string status, string? remarks, int adminId)
        //{
        //    if (status != "Approved" && status != "Rejected")
        //    {
        //        return ServiceResult.Fail("Invalid decision status.");
        //    }

        //    int rowsAffected = await _leaveRepository.UpdateStatusAsync(leaveId, status, remarks, adminId);
        //    if (rowsAffected == 0)
        //    {
        //        return ServiceResult.Fail("Leave request not found or could not be updated.");
        //    }

        //    return ServiceResult.Ok($"Leave request has been marked as {status}.");
        //}

        public async Task<IEnumerable<dynamic>> GetAllLeaveRequestsAsync()
        {
            return await _leaveRepository.GetAllLeavesWithEmployeeDetailsAsync();
        }
        public async Task<List<LeaveRequest>> GetLeavesByUserIdAsync(int userId)
        {
            return await _leaveRepository.GetLeavesByUserIdAsync(userId);
        }
       

        async Task<bool> ILeaveService.ReviewLeaveRequestAsync(int leaveId, string status, string? remarks, int adminId)
        {
            int result= await _leaveRepository.UpdateStatusAsync(leaveId, status, remarks, adminId);

            return result > 0;
        }
    }
}
