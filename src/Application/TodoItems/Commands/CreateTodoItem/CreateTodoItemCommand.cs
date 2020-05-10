using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Common.Attributes;
using CleanArchitecture.Application.Common.Events;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem
{
    [CacheInvalidate(CachedQuery = typeof(ExportTodosQuery),
        PropertiesUsedForCacheKey = CacheConstants.ExportTodosQueryPropertyCacheKey)]
    public class CreateTodoItemCommand : IRequest<int>
    {
        public int ListId { get; set; }
        public string Title { get; set; }
    }

    public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateTodoItemCommandHandler(IApplicationDbContext context,
            IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoItem
            {
                ListId = request.ListId,
                Title = request.Title,
                Done = false
            };

            await _context.TodoItems.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new TodoNotification()
            {
                EventType = TodoEvent.Created,
                TodoItem = entity
            }, cancellationToken);

            return entity.Id;
        }
    }
}
