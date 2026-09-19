using LMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSM.Persistence.Interface
{
    public interface ILeaveRepository
    {
        Task<bool> HasOverlappingLeaveAsync(int userId, DateTime fromDate, DateTime toDate);
        Task<int> CreateLeaveRequestAsync(LeaveRequest request);
        Task<IEnumerable<LeaveRequest>> GetLeavesByUserIdAsync(int userId);
        Task<IEnumerable<dynamic>> GetAllLeavesWithEmployeeDetailsAsync();
        Task<int> UpdateStatusAsync(int leaveId, string status, string? remarks, int adminId);
        Task<(int Pending, int Approved, int Rejected)> GetSummaryCountsAsync();
    }
}
