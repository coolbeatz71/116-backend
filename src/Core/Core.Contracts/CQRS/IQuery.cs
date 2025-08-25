using MediatR;

namespace _116.Core.Contracts.CQRS;

/// <summary>
/// Represents a query that returns a response of type <typeparamref name="TResponse"/>.
/// Extends <see cref="IRequest{TResponse}"/> from MediatR.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned when the query is handled.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : notnull;