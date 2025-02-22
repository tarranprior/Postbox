using Postbox.Communication;
using Serilog;

namespace Postbox.KeyManagement;

/// <summary>
/// Provides functionality for exchanging public keys.
/// </summary>
public static class KeyExchange
{
    /// <summary>
    /// Sends a public key to the specified email address.
    /// </summary>
    /// <param name="key">The file path of the public key.</param>
    /// <param name="email">The recipient's email address.</param>
    public static async Task SendPublicKey(string key, string email)
    {
        try
        {
            string publicKeyContent = File.ReadAllText(key);
            string subject = $"Postbox Key {DateTime.UtcNow:dd-MM-yyyy-HH:mm:ss}";
            string body = $"-----BEGIN RSA PUBLIC KEY-----\n{publicKeyContent}\n-----END RSA PUBLIC KEY-----";
            await SmtpHandler.SendEmail(email, subject, body, key);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    /// <summary>
    /// Sends a message to the specified email address.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="email">The recipient's email address.</param>
    public static async Task SendMessage(string message, string email)
    {
        try
        {
            string subject = $"Postbox Message {DateTime.UtcNow:dd-MM-yyyy-HH:mm:ss}";
            string body = $"{message}";
            await SmtpHandler.SendEmail(email, subject, body);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }
}