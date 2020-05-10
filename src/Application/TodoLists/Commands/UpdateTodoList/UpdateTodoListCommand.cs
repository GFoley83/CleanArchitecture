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

namespace CleanArchitecture.Application.TodoLists.Commands.UpdateTodoList
{
    [CacheInvalidate(CachedQuery = typeof(ExportTodosQuery),
        PropertiesUsedForCacheKey = CacheConstants.ExportTodosQueryPropertyCacheKey,
        MatchToProperties = "Id")]
    public class UpdateTodoListCommand : IRequest
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }

    public class UpdateTodoListCommandHandler : IRequestHandler<UpdateTodoListCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public UpdateTodoListCommandHandler(IApplicationDbContext context,
            IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.TodoLists.FindAsync(request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(TodoList), request.Id);
            }

            entity.Title = request.Title;

            await _context.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new TodoListNotification()
            {
                EventType = TodoListEvent.Updated,
                TodoList = entity
            }, cancellationToken);

            return Unit.Value;
        }
    }
}
