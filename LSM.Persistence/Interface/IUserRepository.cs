using LMS.Domain;

namespace LSM.Persistence.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<dynamic>> GetEmployeesWithLeaveCountAsync(string? searchTerm);
        Task<int> ToggleEmployeeStatusAsync(int id);
        Task<int> GetTotalEmployeesCountAsync();
        Task<List<User?>> GetAllUserAsync();
    }
}
