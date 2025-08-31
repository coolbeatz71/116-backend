using MediatR;

namespace _116.Shared.Domain;

/// <summary>
/// A domain event that can be handled via MediatR.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// Gets the unique identifier for the domain event.
    /// </summary>
    Guid EventId => Guid.NewGuid();

    /// <summary>
    /// Gets the timestamp when the domain event was created.
    /// </summary>
    public DateTime CreatedAt => DateTime.Now;

    /// <summary>
    /// Gets the fully qualified name of the event type.
    /// </summary>
    public string EventType => GetType().AssemblyQualifiedName!;
}
