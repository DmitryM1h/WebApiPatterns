using CriticalEvents.Domain.Enums;



namespace CriticalEvents.Domain.Entities
{
    public record Accident
    {
        public Guid id { get; init; }
        public AccidentType Type { get; init; }
        public CriticalEvent CriticalEventFirst { get; init; } = null!;
        public CriticalEvent? CriticalEventSecond { get; init; }


        public Accident(Guid id, AccidentType type, CriticalEvent criticalEventFirst, CriticalEvent? criticalEventSecond = null)
        {
            this.id = id;
            this.Type = type;
            this.CriticalEventFirst = criticalEventFirst;
            this.CriticalEventSecond = criticalEventSecond;
        }
    }
}
