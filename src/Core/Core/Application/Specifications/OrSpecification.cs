using System.Linq.Expressions;

namespace Core.Application.Specifications;

/// <summary>
/// Combines two specifications using logical OR.
/// </summary>
public class OrSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
{
    /// <summary>
    /// Combines two specification expressions using logical OR.
    /// </summary>
    /// <returns>An expression that evaluates to true when either left or right specification is satisfied.</returns>
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpr = left.ToExpression();
        var rightExpr = right.ToExpression();

        var param = Expression.Parameter(typeof(T));
        var body = Expression.OrElse(
            Expression.Invoke(leftExpr, param),
            Expression.Invoke(rightExpr, param));

        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}