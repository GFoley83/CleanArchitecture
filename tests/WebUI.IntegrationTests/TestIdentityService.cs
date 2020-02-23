using CleanArchitecture.Application;
using CleanArchitecture.Application.Common.Models;
using System;
using System.Threading.Tasks;

namespace CleanArchitecture.WebUI.IntegrationTests
{
    public class TestIdentityService : IIdentityService
    {
        public Task<string> GetUserNameAsync(string userId)
        {
            return Task.FromResult("jason@clean-architecture");
        }

        public Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string role = null)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UserIsInRoleAsync(string userId, string role)
        {
            throw new NotImplementedException();
        }
    }
}
