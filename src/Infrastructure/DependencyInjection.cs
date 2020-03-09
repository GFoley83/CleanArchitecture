using CleanArchitecture.Application;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Files;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Services;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace CleanArchitecture.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());



            //services
            //    .AddDefaultIdentity<ApplicationUser>(options =>
            //    {
            //        // Basic built in validations
            //        options.Password.RequireDigit = false;
            //        options.Password.RequireLowercase = true;
            //        options.Password.RequireNonAlphanumeric = true;
            //        options.Password.RequireUppercase = true;
            //        options.Password.RequiredLength = 6;
            //    })
            //    .AddRoles<IdentityRole>()
            //    .AddRoleManager<RoleManager<IdentityRole>>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Basic built in validations
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
            })
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            if (environment.IsEnvironment("Test"))
            {
                services.AddIdentityServer()
                    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
                    {
                        options.Clients.Add(new Client
                        {
                            ClientId = "CleanArchitecture.IntegrationTests",
                            AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
                            ClientSecrets = { new Secret("secret".Sha256()) },
                            AllowedScopes = { "CleanArchitecture.WebUIAPI", "openid", "profile" }
                        });
                    }).AddTestUsers(new List<TestUser>
                    {
                        new TestUser
                        {
                            SubjectId = "f26da293-02fb-4c90-be75-e4aa51e0bb17",
                            Username = "jason@clean-architecture",
                            Password = "CleanArchitecture!",
                            Claims = new List<Claim>
                            {
                                new Claim(JwtClaimTypes.Email, "jason@clean-architecture"),
                                new Claim(JwtClaimTypes.Role, "GlobalAdmin")
                            }
                        }
                    });
            }
            else
            {
                services.AddIdentityServer()
                    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(cfg =>
                    {
                        //cfg.ApiResources.First().UserClaims.Add(ClaimTypes.Role);
                        //cfg.ApiResources.First().UserClaims.Add(JwtClaimTypes.Role);

                        //cfg.IdentityResources.Add(new IdentityResource { Name = "role", UserClaims = new List<string> { "role" } });
                        //cfg.IdentityResources.Add(new IdentityResource { Name = "roles", UserClaims = new List<string> { "role" } });
                      
                        //cfg.Clients.First().AllowedScopes = new List<string>
                        //{
                        //    IdentityServerConstants.StandardScopes.OpenId,
                        //    IdentityServerConstants.StandardScopes.Profile,
                        //    "roles",
                        //    "role"
                        //};
                    });

                services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppUserClaimsPrincipalFactory>();

                services.AddTransient<IDateTime, DateTimeService>();
                services.AddTransient<IIdentityService, IdentityService>();
                services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();
            }


            services.AddAuthentication()
                .AddIdentityServerJwt();

            return services;
        }
    }
}
