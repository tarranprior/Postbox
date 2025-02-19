using Postbox.Communication;
using Serilog;

namespace Postbox.KeyManagement;

/// <summary>
/// Provides functionality for exchanging public keys.
/// </summary>
public static class KeyExchange
{
    /// <summary>
    /// Sends the public key to the specified email address.
    /// </summary>
    /// <param name="key">The file path of the public key.</param>
    /// <param name="email">The recipient's email address.</param>
    public static void SendPublicKey(string key, string email)
    {
        try
        {
            string publicKeyContent = File.ReadAllText(key);
            string subject = $"Postbox Key {DateTime.UtcNow:dd-MM-yyyy-HH:mm:ss}";
            string body = $"-----BEGIN RSA PUBLIC KEY-----\n{publicKeyContent}\n-----END RSA PUBLIC KEY-----";
            SmtpHandler.SendEmail(email, subject, body, key);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }
}