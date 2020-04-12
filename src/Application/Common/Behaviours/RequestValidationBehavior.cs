using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValidationException = CleanArchitecture.Application.Common.Exceptions.ValidationException;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var validator = _validators.GetCommandValidator();

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

            throw new ValidationException(failures);
        }
    }


    public static class FluentValidationExt
    {
        public static IValidator<TRequest> GetCommandValidator<TRequest>(this IEnumerable<IValidator<TRequest>> validators)
            => validators.SingleOrDefault(i => !(i is IAuthValidator));

        public static IValidator<TRequest> GetAuthValidator<TRequest>(this IEnumerable<IValidator<TRequest>> validators)
            => validators.SingleOrDefault(i => i is IAuthValidator);
    }
}