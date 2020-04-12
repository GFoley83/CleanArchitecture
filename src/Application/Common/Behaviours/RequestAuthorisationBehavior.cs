using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class RequestAuthorisationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public RequestAuthorisationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var validator = _validators.GetAuthValidator();

            if (validator == null)
            {
                return next();
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