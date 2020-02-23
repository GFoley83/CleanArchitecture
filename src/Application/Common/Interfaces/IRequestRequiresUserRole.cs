using MediatR;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IRequestRequiresUserRole<out TResponse> : IRequest<TResponse>
    {
        string RequiredRole { get; }
    }
}
