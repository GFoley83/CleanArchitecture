using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace CleanArchitecture.Application.Common.Validators
{
    public class RequestValidator<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public RequestValidator(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;

            if (!_validators.Any())
            {
                return;
            }

            var context = new ValidationContext(request);

            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        }
    }
}
