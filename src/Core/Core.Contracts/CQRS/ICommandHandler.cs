using MediatR;

namespace _116.Core.Contracts.CQRS;

/// <summary>
/// Handles commands that do not return a response.
/// This interface inherits from <see cref="ICommandHandler{TCommand, Unit}"/>.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle, which returns no result.</typeparam>
public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>
    where TCommand : ICommand<Unit>;

/// <summary>
/// Handles commands that return a response of type <typeparamref name="TResponse"/>.
/// Extends <see cref="IRequestHandler{TRequest, TResponse}"/> from MediatR.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the handler.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull;