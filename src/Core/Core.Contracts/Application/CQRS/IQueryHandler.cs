using MediatR;

namespace _116.Core.Contracts.Application.CQRS;

/// <summary>
/// Handles queries of type <typeparamref name="TQuery"/> that return a response of type <typeparamref name="TResponse"/>.
/// Extends <see cref="IRequestHandler{TRequest, TResponse}"/> from MediatR.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the handler.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull;
