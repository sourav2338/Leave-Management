using LMS.Application.Interface;
using LMS.Domain;
using LSM.Persistence.Interface;


namespace LMS.Application.Service
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !user.IsActive)
            {
                return null;
            }
            string hash = PasswordHelper.HashPassword(password);
            if (user.PasswordHash != hash)
            {
                return null;
            }

            return user;
        }

        public Task<List<User>> GetEmployeesListAsync(string? searchTerm)
        {
            throw new NotImplementedException();
        }
        public async Task<List<User?>> GetAllEmployeeAsync()
        {
            var user =await _userRepository.GetAllUserAsync();
           
            return user;
        }

        //public async Task<ServiceResult> ToggleEmployeeStatusAsync(int employeeId)
        //{
        //    var user = await _userRepository.GetByIdAsync(employeeId);
        //    if (user == null)
        //    {
        //        return ServiceResult.Fail("Employee record not found.");
        //    }

        //    if (user.Role == "Admin")
        //    {
        //        return ServiceResult.Fail("Cannot deactivate an administrator account.");
        //    }

        //    await _userRepository.ToggleEmployeeStatusAsync(employeeId);
        //    string newStatus = user.IsActive ? "deactivated" : "activated";
        //    return ServiceResult.Ok($"Employee account successfully {newStatus}.");
        //}


    }
}
