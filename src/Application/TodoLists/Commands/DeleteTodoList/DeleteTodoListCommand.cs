using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Common.Attributes;
using CleanArchitecture.Application.Common.Events;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoLists.Commands.DeleteTodoList
{
    [CacheInvalidate(CachedQuery = typeof(ExportTodosQuery),
        PropertiesUsedForCacheKey = CacheConstants.ExportTodosQueryPropertyCacheKey,
        MatchToProperties = "Id")]
    public class DeleteTodoListCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteTodoListCommandHandler : IRequestHandler<DeleteTodoListCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public DeleteTodoListCommandHandler(IApplicationDbContext context,
            IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.TodoLists
                .Where(l => l.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(TodoList), request.Id);
            }

            _context.TodoLists.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new TodoListNotification()
            {
                EventType = TodoListEvent.Deleted,
                TodoList = entity
            }, cancellationToken);

            return Unit.Value;
        }
    }
}
