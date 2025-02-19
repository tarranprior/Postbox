using System.Text.RegularExpressions;
using Serilog;

namespace Postbox.Utilities;

public static class Validation
{
    /// <summary>
    /// Validates specified email addresses.
    /// </summary>
    /// /// <param name="email">The email address to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    public static bool ValidateEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return false;
        }

        bool isValid = Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return isValid;
    }
}