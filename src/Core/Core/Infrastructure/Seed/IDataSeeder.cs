namespace _116.Core.Infrastructure.Seed;

/// <summary>
/// Defines a contract for seeding initial data into the application's data store.
/// </summary>
public interface IDataSeeder
{
    /// <summary>
    /// Executes all configured seed operations asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous seeding operation.</returns>
    Task SeedAllAsync();
}
