using CleanArchitecture.Application.Common.Extensions;
using CleanArchitecture.Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class AuthorisationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ICurrentUserService _currentUserService;

        public AuthorisationBehavior(IEnumerable<IValidator<TRequest>> validators,
            ICurrentUserService currentUserService)
        {
            _validators = validators;
            _currentUserService = currentUserService;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var validator = _validators.GetAuthValidator();

            if (validator == null)
            {
                return next();
            }

            if (string.IsNullOrEmpty(_currentUserService.UserId))
            {
                throw new AuthenticationException("User must be signed in.");
            }

            var context = new ValidationContext(request);

            var failures = validator
                .Validate(context)
                .Errors
                .Where(f => f != null)
                .ToList();

            if (failures.Count == 0)
            {
                return next();
            }

            throw new UnauthorizedAccessException(string.Join("", failures));
        }
    }
}