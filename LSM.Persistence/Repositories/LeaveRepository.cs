using Dapper;
using LMS.Domain;
using LSM.Persistence.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSM.Persistence.Repositories
{
    public class LeaveRepository:ILeaveRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public LeaveRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> HasOverlappingLeaveAsync(int userId, DateTime fromDate, DateTime toDate)
        {            
            const string sql = @"
                SELECT COUNT(1) 
                FROM LeaveRequests 
                WHERE UserId = @UserId 
                  AND Status != 'Rejected'
                  AND @FromDate <= ToDate 
                  AND @ToDate >= FromDate";

            using var connection = _connectionFactory.CreateConnection();
            int count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, FromDate = fromDate, ToDate = toDate });
            return count > 0;
        }

        public async Task<int> CreateLeaveRequestAsync(LeaveRequest request)
        {
            const string sql = @"
                INSERT INTO LeaveRequests (UserId, FromDate, ToDate, Reason, Status, CreatedOn,CreatedBy)
                VALUES (@UserId, @FromDate, @ToDate, @Reason, @Status, @CreatedOn,@CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, request);
        }

        public async Task<List<LeaveRequest>> GetLeavesByUserIdAsync(int userId)
        {
            const string sql = @"
                SELECT Id, UserId, FromDate, ToDate, Reason, Status, AdminRemarks, ReviewedByAdminId, ReviewedAt, CreatedOn
                FROM LeaveRequests
                WHERE UserId = @UserId
                ORDER BY CreatedOn DESC";

            using var connection = _connectionFactory.CreateConnection();
            var result= await connection.QueryAsync<LeaveRequest>(sql, new { UserId = userId });
            return result.AsList();
        }

        public async Task<IEnumerable<dynamic>> GetAllLeavesWithEmployeeDetailsAsync()
        {
            const string sql = @"
                SELECT 
                    lr.Id,
                    lr.UserId,
                    u.FullName AS EmployeeName,
                    u.Email AS EmployeeEmail,
                    lr.FromDate,
                    lr.ToDate,
                    lr.Reason,
                    lr.Status,
                    lr.AdminRemarks,
                    lr.ReviewedAt,
                    lr.CreatedOn
                FROM LeaveRequests lr
                INNER JOIN Users u ON lr.UserId = u.Id
                ORDER BY lr.CreatedOn DESC";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync(sql);
        }

        public async Task<int> UpdateStatusAsync(int leaveId, string status, string? remarks, int adminId)
        {
            const string sql = @"
                UPDATE LeaveRequests 
                SET Status = @Status,
                    AdminRemarks = @Remarks,
                    ReviewedByAdminId = @AdminId,
                    ReviewedAt = @ReviewedAt
                WHERE Id = @Id";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync(sql, new
            {
                Id = leaveId,
                Status = status,
                Remarks = remarks,
                AdminId = adminId,
                ReviewedAt = DateTime.UtcNow
            });
        }

        public async Task<(int Pending, int Approved, int Rejected)> GetSummaryCountsAsync()
        {
            const string sql = @"
                SELECT 
                    SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) AS Pending,
                    SUM(CASE WHEN Status = 'Approved' THEN 1 ELSE 0 END) AS Approved,
                    SUM(CASE WHEN Status = 'Rejected' THEN 1 ELSE 0 END) AS Rejected
                FROM LeaveRequests";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync(sql);
            return (result?.Pending ?? 0, result?.Approved ?? 0, result?.Rejected ?? 0);
        }
    }
}
