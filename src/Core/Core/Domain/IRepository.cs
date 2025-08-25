namespace _116.Core.Domain;

/// <summary>
/// Generic repository interface for managing aggregates in the domain layer.
/// </summary>
/// <typeparam name="T">The aggregate type that implements <see cref="IAggregate"/>.</typeparam>
public interface IRepository<T> where T : IAggregate;
