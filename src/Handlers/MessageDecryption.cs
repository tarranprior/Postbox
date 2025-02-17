using System.Security.Cryptography;
using System.Text;

namespace Postbox.Handlers;

public static class MessageDecryption
{
    public static string Decrypt(string message, string? privateKeyPath = null)
    {
        try
        {
            byte[] privateKeyBytes = Convert.FromBase64String(File.ReadAllText(privateKeyPath));
            byte[] encryptedMessageBytes = Convert.FromBase64String(message);
            
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

            byte[] decryptedMessageBytes = rsa.Decrypt(encryptedMessageBytes, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decryptedMessageBytes);
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }
}