using CleanArchitecture.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common
{
    public abstract class AuthValidator<T> : AbstractValidator<T>, IAuthValidator
    {
        public ICurrentUserService CurrentUserService { get; }

        protected AuthValidator(ILogger<AuthValidator<T>> logger,
            ICurrentUserService currentUserService)
        {
            CurrentUserService = currentUserService;

            //RuleFor(v => v)
            //    .Cascade(CascadeMode.StopOnFirstFailure)
            //    .Must((_) =>
            //    {
            //        // If is logged in
            //        if (!string.IsNullOrEmpty(CurrentUserService.UserId))
            //        {
            //            return true;
            //        }

            //        logger.LogWarning("User is not signed in.");
            //        return false;
            //    })
            //    .WithMessage("User must be signed in.");
        }
    }
}