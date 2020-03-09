using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public AppUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            var roles = await UserManager.GetRolesAsync(user);
            
            // Continue if "role" is already set
            if (identity.Claims.Any(i => i.Type == JwtClaimTypes.Role && string.IsNullOrWhiteSpace(i.Value) == false))
            {
                return identity;
            }

            identity.AddClaims(roles.Select(r => new Claim(JwtClaimTypes.Role, r)));

            return identity;
        }
    }
}