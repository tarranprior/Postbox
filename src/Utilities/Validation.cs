using System.Text.RegularExpressions;

namespace Postbox.Utilities;

public static partial class Validation
{
    /// <summary>
    /// Validates specified email addresses.
    /// </summary>
    /// /// <param name="email">The email address to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    public static bool IsEmail(string email)
    {
        return EmailRegex().IsMatch(email);
    }

    /// <summary>
    /// Precompiled regex for validating email addresses using .NET 7+ GeneratedRegexAttribute.
    /// </summary>
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();

    /// <summary>
    /// Checks whether the provided message is an encrypted message.
    /// </summary>
    /// <param name="message">The message to validate.</param>
    /// <param name="keySize">The key size used for encryption (2048, 3072, or 4096).</param>
    /// <returns>True if the message appears to be encrypted, false otherwise.</returns>
    public static bool IsEncryptedMessage(string message, int keySize)
    {
        if (!IsBase64String(message))
        {
            return false;
        }

        try
        {
            byte[] encryptedBytes = Convert.FromBase64String(message);

            int expectedBlockSize = keySize / 8; // i.e., 2048-bit key -> 256-byte block
            return encryptedBytes.Length == expectedBlockSize;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether a given string is a valid Base64-encoded string.
    /// </summary>
    [GeneratedRegex(@"^[a-zA-Z0-9\+/]*={0,2}$")]
    private static partial Regex Base64Regex();

    public static bool IsBase64String(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length % 4 != 0)
        {
            return false;
        }

        return Base64Regex().IsMatch(input);
    }
}