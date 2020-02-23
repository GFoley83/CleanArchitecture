using CleanArchitecture.Application.Common;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new ApplicationUser { UserName = "jason@clean-architecture", Email = "jason@clean-architecture" };

            if (!await roleManager.RoleExistsAsync(UserRole.GlobalAdmin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRole.GlobalAdmin));
            }

            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
            {
                await userManager.CreateAsync(defaultUser, "CleanArchitecture!");

                defaultUser = await userManager.FindByNameAsync(defaultUser.UserName);
                await userManager.AddToRoleAsync(defaultUser, UserRole.GlobalAdmin);
                await userManager.AddClaimAsync(defaultUser, new Claim(ClaimTypes.Role, UserRole.GlobalAdmin));
            }
        }
    }
}
