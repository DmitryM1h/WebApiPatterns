using CriticalEvents.Domain.Enums;
using CriticalEvents.Domain.Exceptions;
using CriticalEvents.Domain.Services.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace CriticalEvents.Domain.Entities
{

    public record CriticalEvent
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public CriticalEventType Type { get; set; }
        public DateTime Date { get; set; } 

        public CriticalEvent(Guid id, string description, CriticalEventType type, DateTime date)
        {
            Id = id;
            Description = description;
            Type = type;
            Date = date;
        }

        public static CriticalEvent Create(CriticalEventRequest @event)
        {
            if (!Enum.IsDefined(@event.Type))
                throw new EventValidationException();

            return new CriticalEvent(Guid.NewGuid(), @event.Description, @event.Type, DateTime.Now);
        }
    }


}
