using CleanArchitecture.Application.Common.Events;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoLists.Commands.CreateTodoList
{
    public partial class CreateTodoListCommand : IRequest<int>
    {
        public string Title { get; set; }
    }

    public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateTodoListCommandHandler(IApplicationDbContext context,
            IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<int> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoList();

            entity.Title = request.Title;

            await _context.TodoLists.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new TodoListNotification()
            {
                EventType = TodoListEvent.Created,
                TodoList = entity
            }, cancellationToken);

            return entity.Id;
        }
    }
}
