using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Common.Events;
using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoLists.Queries
{
    public class ExportTodoListCacheInvalidationNotificationHandler : INotificationHandler<TodoListNotification>
    {
        private readonly ICacheService _cacheService;

        public ExportTodoListCacheInvalidationNotificationHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public Task Handle(TodoListNotification notification, CancellationToken cancellationToken)
        {
            // There's nothing to invalidate for newly created TodoLists
            if (notification.EventType == TodoListEvent.Created)
            {
                return Task.CompletedTask;
            }

            // Or we could use 
            var cacheKey = CacheHelper.GenerateKey(typeof(ExportTodosQuery),
                CacheConstants.ExportTodosQueryPropertyCacheKey,
                notification.TodoList.Id.ToString());

            _cacheService.RemoveCacheValue(cacheKey);

            return Task.CompletedTask;
        }
    }
}