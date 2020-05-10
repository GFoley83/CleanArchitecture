using CleanArchitecture.Application.Common.Events;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Common.Models
{
    public class TodoNotification : INotification
    {
        public TodoEvent EventType { get; set; }
        public TodoItem TodoItem { get; set; }
    }
}