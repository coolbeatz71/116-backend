using _116.User.Application.Shared.Authorizations.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace _116.User.Application.Shared.Authorizations.Handlers;

/// <summary>
/// Authorization handler that validates account status requirements against user claims.
/// </summary>
/// <remarks>
/// Checks if the user's JWT token contains the required claim with the expected value.
/// Used for enforcing account status policies like verification, active status, etc.
/// </remarks>
public class AccountStatusRequirementHandler : AuthorizationHandler<AccountStatusRequirement>
{
    /// <summary>
    /// Evaluates the account status requirement against the current authorization context.
    /// </summary>
    /// <param name="context">The authorization context containing user claims</param>
    /// <param name="requirement">The account status requirement specifying claim type and value</param>
    /// <returns>A completed task representing the authorization evaluation</returns>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AccountStatusRequirement requirement
    )
    {
        // Extract the claim value from the user's JWT token
        string? claimValue = context.User.FindFirst(requirement.ClaimType)?.Value;

        if (string.IsNullOrEmpty(claimValue))
        {
            return Task.CompletedTask;
        }

        // Authorize if the claim exists and matches the required value
        if (claimValue.Equals(requirement.ClaimValue, StringComparison.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
