using System.Reflection;
using _116.Core.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace _116.Core.Application.Extensions;

/// <summary>
/// Extension methods for registering MediatR and related pipeline behaviors.
/// </summary>
public static class MediatRExtension
{
    /// <summary>
    /// Adds MediatR services and registers validation and logging pipeline behaviors for the specified assemblies.
    /// Also registers all FluentValidation validators found in the provided assemblies.
    /// </summary>
    /// <param name="services">The service collection to which the MediatR services are added.</param>
    /// <param name="assemblies">The assemblies to scan for MediatR handlers and FluentValidation validators.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    /// <remarks>
    /// This method performs the following:
    /// <list type="bullet">
    /// <item><description>Registers MediatR handlers from the provided assemblies.</description></item>
    /// <item><description>Adds <see cref="ValidationBehavior{TRequest,TResponse}"/> to MediatR pipeline.</description></item>
    /// <item><description>Adds <see cref="LoggingBehavior{TRequest, TResponse}"/> to MediatR pipeline.</description></item>
    /// <item><description>Registers FluentValidation validators from the same assemblies.</description></item>
    /// </list>
    ///
    /// <para>Example usage:</para>
    /// <code>
    /// services.AddMediatRWithAssemblies(typeof(SomeModule).Assembly, typeof(AnotherModule).Assembly);
    /// </code>
    /// </remarks>
    public static IServiceCollection AddMediatRWithAssemblies(
        this IServiceCollection services,
        params Assembly[] assemblies
    )
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assemblies);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssemblies(assemblies);

        return services;
    }
}
