using FluentValidation;

namespace _116.Core.Application.Extensions;

/// <summary>
/// Provides custom FluentValidation extensions for commonly used validation rules.
/// </summary>
public static class ValidationExtension
{
    /// <summary>
    /// Adds a validation rule to ensure that a string property is a non-empty, valid GUID.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="ruleBuilder">The FluentValidation rule builder for the property.</param>
    /// <param name="fieldDisplayName">Optional display name used in validation messages (defaults to "Id").</param>
    /// <example>
    /// <code>
    /// RuleFor(x => x.UserId).IsValidGuid("User ID");
    /// </code>
    /// </example>
    public static void IsValidGuid<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string fieldDisplayName = "Id"
    )
    {
        ruleBuilder
            .NotEmpty().WithMessage($"{fieldDisplayName} is required.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage($"{fieldDisplayName} is invalid.");
    }
}
