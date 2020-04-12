using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.TodoLists.Commands.CreateTodoList
{
public interface IAuthValidator { }

public abstract class AuthValidator<T> : AbstractValidator<T>, IAuthValidator
{
    public ICurrentUserService CurrentUserService { get; }

    protected AuthValidator(ILogger<AuthValidator<T>> logger,
        ICurrentUserService currentUserService)
    {
        CurrentUserService = currentUserService;

        RuleFor(v => v)
            .Must((x, c) =>
            {
                // If is logged in
                if (!string.IsNullOrEmpty(CurrentUserService.UserId))
                {
                    return true;
                }

                logger.LogWarning("User is not signed in.");
                return false;
            })
            .WithMessage("User must be signed in.");
    }
}

public class CreateTodoListAuthValidator : AuthValidator<CreateTodoListCommand>
{
    public CreateTodoListAuthValidator(ILogger<CreateTodoListAuthValidator> logger,
        ICurrentUserService currentUserService,
        IIdentityService identityService) : base(logger, currentUserService)
    {
        RuleFor(v => v)
            .MustAsync(async (x, c) =>
            {
                var userIsInRoleResult = await identityService.UserIsInRoleAsync(currentUserService.UserId, UserRole.GlobalAdmin);

                if (!userIsInRoleResult.Succeeded)
                {
                    logger.LogWarning($"User: {currentUserService.UserId} is not in role: {UserRole.GlobalAdmin}.");
                    return false;
                }

                return userIsInRoleResult.Succeeded;
            })
            .WithMessage($"User does not have permission to create todo lists.");
    }
}
}