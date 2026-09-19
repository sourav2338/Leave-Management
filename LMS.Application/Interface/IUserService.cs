using LMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Interface
{
    public interface IUserService
    {
        Task<User?> ValidateUserCredentialsAsync(string email, string password);
        //Task<bool> ToggleEmployeeStatusAsync(int employeeId);
        Task<List<User>> GetEmployeesListAsync(string? searchTerm);
        Task<List<User?>> GetAllEmployeeAsync();
    }
}
