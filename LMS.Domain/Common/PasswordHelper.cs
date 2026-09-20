using System.Security.Cryptography;
using System.Text;

namespace LMS.Domain.Common
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = SHA256.HashData(inputBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
