using Eventia.Domain.Common;

namespace Eventia.Domain.Entities;

public class Event : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime EventDate { get; private set; }
    public string Location { get; private set; } = string.Empty;
    public Guid CreatedById { get; private set; }

    public User? CreatedBy { get; private set; }
    public ICollection<Ticket> Tickets { get; private set; } = new List<Ticket>();

    protected Event() { }

    public static Event Create(string name, string description, DateTime eventDate, string location, Guid createdById)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Event name is required.");
        return new Event
        {
            Name = name,
            Description = description,
            EventDate = eventDate,
            Location = location,
            CreatedById = createdById
        };
    }

    public void Update(string name, string description, DateTime eventDate, string location)
    {
        Name = name;
        Description = description;
        EventDate = eventDate;
        Location = location;
        Touch();
    }
}
