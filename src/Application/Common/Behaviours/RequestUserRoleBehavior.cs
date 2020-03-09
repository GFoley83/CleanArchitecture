using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class RequestUserRoleBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IRequestRequiresUserRole<TResponse>
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public RequestUserRoleBehavior(ILogger<TRequest> logger,
            ICurrentUserService currentUserService,
            IIdentityService identityService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!(request is IRequestRequiresUserRole<TResponse>))
            {
                return await next();
            }

            var userRoleRequest = request as IRequestRequiresUserRole<TResponse>;
            var userIsInRoleResult = await _identityService.UserIsInRoleAsync(_currentUserService.UserId, userRoleRequest.RequiredRole);

            if (!userIsInRoleResult.Succeeded)
            {
                _logger.LogWarning($"User: {_currentUserService.UserId} is not in role: {userRoleRequest.RequiredRole}.");
                throw new UnauthorizedAccessException(string.Join(". ", userIsInRoleResult.Errors));
            }

            return await next();
        }
    }
}
