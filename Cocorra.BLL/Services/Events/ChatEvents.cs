using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cocorra.BLL.Services.Events
{
    public class ChatEvents
    {
        public record MessagesReadEvent(Guid ReaderId, Guid SenderId) : INotification;
    }
}

