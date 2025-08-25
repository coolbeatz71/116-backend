using MediatR;

namespace _116.Core.Contracts.CQRS;

/// <summary>
/// Represents a command that does not return a result.
/// This is a marker interface inheriting from <see cref="ICommand{Unit}"/>.
/// </summary>
public interface ICommand : ICommand<Unit>;

/// <summary>
/// Represents a command with a response of type <typeparamref name="TResult"/>.
/// Extends <see cref="IRequest{TResponse}"/> from MediatR.
/// </summary>
/// <typeparam name="TResult">The type of the response returned when the command is handled.</typeparam>
public interface ICommand<out TResult> : IRequest<TResult>;