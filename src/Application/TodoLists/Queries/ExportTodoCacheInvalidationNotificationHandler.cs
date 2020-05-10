using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoLists.Queries
{
    public class ExportTodoCacheInvalidationNotificationHandler : INotificationHandler<TodoNotification>
    {
        private readonly ICacheService _cacheService;

        public ExportTodoCacheInvalidationNotificationHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public Task Handle(TodoNotification notification, CancellationToken cancellationToken)
        {
            var cacheKey = CacheHelper.GenerateKey(typeof(ExportTodosQuery),
                CacheConstants.ExportTodosQueryPropertyCacheKey,
                notification.TodoItem.ListId.ToString());

            _cacheService.RemoveCacheValue(cacheKey);

            return Task.CompletedTask;
        }
    }
}