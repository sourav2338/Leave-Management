
using LMS.Domain;
using LMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Interface
{
    public interface ILeaveService
    {
        
        
        //Task<AdminDashboardViewModel> GetAdminDashboardAsync();
        Task<ServiceResult> ApplyLeaveAsync(int userId, LeaveRequest model);
        Task<bool> ReviewLeaveRequestAsync(int leaveId, string status, string? remarks, int adminId);
        Task<IEnumerable<dynamic>> GetAllLeaveRequestsAsync();
        Task<List<LeaveRequest>> GetLeavesByUserIdAsync(int userId);
    }
}
