using System.Security.Cryptography;
using System.Text;

namespace Postbox.Handlers;

public static class MessageEncryption
{
    public static string Encrypt(string message, string publicKeyPath)
    {
        try
        {
            byte[] publicKeyBytes = Convert.FromBase64String(File.ReadAllText(publicKeyPath));

            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(publicKeyBytes, out _);

            byte[] encryptedMessageBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encryptedMessageBytes);
            
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }
}