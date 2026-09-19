using LMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LSM.Persistence.Interface;

namespace LSM.Persistence.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            const string sql = @"
                SELECT Id, FullName, Email, PasswordHash, Role, IsActive 
                FROM Users 
                WHERE Email = @Email";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT Id, FullName, Email, PasswordHash, Role, IsActive, CreatedAt 
                FROM Users 
                WHERE Id = @Id";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<IEnumerable<dynamic>> GetEmployeesWithLeaveCountAsync(string? searchTerm)
        {
            const string sql = @"
                SELECT 
                    u.Id, 
                    u.FullName, 
                    u.Email, 
                    u.IsActive, 
                    u.CreatedAt,
                    COUNT(lr.Id) AS TotalLeavesCount
                FROM Users u
                LEFT JOIN LeaveRequests lr ON u.Id = lr.UserId
                WHERE u.Role = 'Employee'
                  AND (@Search IS NULL OR u.FullName LIKE '%' + @Search + '%' OR u.Email LIKE '%' + @Search + '%')
                GROUP BY u.Id, u.FullName, u.Email, u.IsActive, u.CreatedAt
                ORDER BY u.FullName ASC";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync(sql, new { Search = searchTerm });
        }

        public async Task<int> ToggleEmployeeStatusAsync(int id)
        {
            const string sql = @"
                UPDATE Users 
                SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END 
                WHERE Id = @Id";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<int> GetTotalEmployeesCountAsync()
        {
            const string sql = "SELECT COUNT(1) FROM Users WHERE Role = 'Employee'";
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql);
        }
        public async Task<List<User?>> GetAllUserAsync()
        {
            const string sql = @"
                SELECT Id, FullName, Email, PasswordHash, Role, IsActive 
                FROM Users ";

            using var connection = _connectionFactory.CreateConnection();
            var users = await connection.QueryAsync<User>(sql);
            return users.AsList();
        }
         
    }
}
