using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Common.Attributes;
using CleanArchitecture.Application.Common.Events;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItemDetail
{
    [CacheInvalidate(CachedQuery = typeof(ExportTodosQuery),
        PropertiesUsedForCacheKey = CacheConstants.ExportTodosQueryPropertyCacheKey)]
    public class UpdateTodoItemDetailCommand : IRequest
    {
        public int Id { get; set; }

        public int ListId { get; set; }

        public PriorityLevel Priority { get; set; }
        public string Note { get; set; }
    }

    public class UpdateTodoItemDetailCommandHandler : IRequestHandler<UpdateTodoItemDetailCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public UpdateTodoItemDetailCommandHandler(IApplicationDbContext context,
            IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(UpdateTodoItemDetailCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.TodoItems.FindAsync(request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(TodoItem), request.Id);
            }

            entity.ListId = request.ListId;
            entity.Priority = request.Priority;
            entity.Note = request.Note;

            await _context.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new TodoNotification()
            {
                EventType = TodoEvent.DetailsUpdated,
                TodoItem = entity
            }, cancellationToken);

            return Unit.Value;
        }
    }
}
