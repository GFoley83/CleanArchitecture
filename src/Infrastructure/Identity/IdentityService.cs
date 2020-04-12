using CleanArchitecture.Application;
using CleanArchitecture.Application.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

            return user.UserName;
        }

        public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string role = null)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!string.IsNullOrWhiteSpace(role))
            {
                user = await _userManager.FindByIdAsync(user.Id);
                await _userManager.AddToRoleAsync(user, role);
            }

            return (result.ToApplicationResult(), user.Id);
        }

        public async Task<Result> DeleteUserAsync(string userId)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return await DeleteUserAsync(user);
            }

            return Result.Success();
        }

        public async Task<Result> UserIsInRoleAsync(string userId, string role)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user == null || !await _userManager.IsInRoleAsync(user, role))
            {
                return Result.Failure(new[] { "User not found or is not in role." });
            }

            return Result.Success();
        }

        public async Task<Result> UserHasClaim(string userId, string type, string value)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user == null || !(await _userManager.GetClaimsAsync(user)).Any(i => i.Type == type && i.Value == value))
            {
                return Result.Failure(new[] { $"User does not have claim \"{user}\"." });
            }

            return Result.Success();
        }

        private async Task<Result> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);

            return result.ToApplicationResult();
        }
    }
}
