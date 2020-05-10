using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Common.Attributes;
using CleanArchitecture.Application.Common.Events;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem
{
    [CacheInvalidate(CachedQuery = typeof(ExportTodosQuery),
        PropertiesUsedForCacheKey = CacheConstants.ExportTodosQueryPropertyCacheKey)]
    public class DeleteTodoItemCommand : IRequest
    {
        public int Id { get; set; }
        public int ListId { get; set; }
    }

    public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public DeleteTodoItemCommandHandler(IApplicationDbContext context,
            IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.TodoItems.FindAsync(request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(TodoItem), request.Id);
            }

            var a =_context.TodoItems.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new TodoNotification()
            {
                EventType = TodoEvent.Deleted,
                TodoItem = entity
            }, cancellationToken);

            return Unit.Value;
        }
    }
}
