using CleanArchitecture.Application.Common.Interfaces;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace CleanArchitecture.Application.Common.Extensions
{
    public static partial class FluentValidationExt
    {
        public static IValidator<TRequest> GetCommandValidator<TRequest>(this IEnumerable<IValidator<TRequest>> validators)
            => validators.SingleOrDefault(i => !(i is IAuthValidator));

        public static IValidator<TRequest> GetAuthValidator<TRequest>(this IEnumerable<IValidator<TRequest>> validators)
            => validators.SingleOrDefault(i => i is IAuthValidator);
    }
}   