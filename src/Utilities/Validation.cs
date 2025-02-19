using System.Text.RegularExpressions;

namespace Postbox.Utilities;

public static partial class Validation
{
    /// <summary>
    /// Validates specified email addresses.
    /// </summary>
    /// /// <param name="email">The email address to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    public static bool ValidateEmail(string email)
    {
        return EmailRegex().IsMatch(email);
    }

    /// <summary>
    /// Precompiled regex for validating email addresses using .NET 7+ GeneratedRegexAttribute.
    /// </summary>
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}