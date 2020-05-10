using CleanArchitecture.Application.Common.Events;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Common.Models
{
    public class TodoListNotification : INotification
    {
        public TodoListEvent EventType { get; set; }
        public TodoList TodoList { get; set; }
    }
}