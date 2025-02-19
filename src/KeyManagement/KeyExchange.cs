using Postbox.Communication;
using Serilog;

namespace Postbox.KeyManagement;

public static class KeyExchange
{
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