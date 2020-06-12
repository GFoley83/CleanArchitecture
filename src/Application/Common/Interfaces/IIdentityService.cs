using CleanArchitecture.Application.Common.Models;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<string> GetUserNameAsync(string userId);

        Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string role = null);

        Task<Result> DeleteUserAsync(string userId);

        Task<Result> UserIsInRoleAsync(string userId, string role);

        Task<Result> UserHasClaim(string userId, string type, string value);
    }
}
