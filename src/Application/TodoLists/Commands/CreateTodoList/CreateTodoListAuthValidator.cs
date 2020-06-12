using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CleanArchitecture.Application.TodoLists.Commands.CreateTodoList
{
    public class CreateTodoListAuthValidator : AuthValidator<CreateTodoListCommand>
    {
        public CreateTodoListAuthValidator(ILogger<CreateTodoListAuthValidator> logger,
            ICurrentUserService currentUserService,
            IIdentityService identityService) : base(logger, currentUserService)
        {
            RuleFor(v => v)
                .MustAsync(async (x, c) =>
                {
                    // var userIsInRoleResult = await identityService.UserIsInRoleAsync(currentUserService.UserId, Constants.UserRoles.GlobalAdmin);
                    var userHasClaim = await identityService.UserHasClaim(currentUserService.UserId, ClaimTypes.Role, Constants.UserRoles.GlobalAdmin);

                    if (!userHasClaim.Succeeded)
                    {
                        logger.LogWarning($"User: {currentUserService.UserId} is not in role: {Constants.UserRoles.GlobalAdmin}.");
                        return false;
                    }

                    return userHasClaim.Succeeded;
                })
                .WithMessage($"User does not have permission to create todo lists.");
        }
    }
}