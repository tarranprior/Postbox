using System.Security.Cryptography;
using System.Text;
using Postbox.Configuration;
using Postbox.KeyManagement;
using Serilog;

namespace Postbox.Handlers;

/// <summary>
/// Provides functionality for decrypting messages using RSA encryption.
/// </summary>
public static class MessageDecryption
{
    /// <summary>
    /// Decrypts a given RSA-encrypted message using a private key file.
    /// </summary>
    /// <param name="message">The Base64-encoded encrypted message.</param>
    /// <param name="privateKeyPath">The file path to the private key file.</param>
    /// <returns>
    /// The decrypted plaintext message, or an empty string if decryption fails.
    /// </returns>
    /// <remarks>
    /// This method uses RSA decryption with OAEP padding and SHA-256 hashing.
    /// The specified private key must be a valid Base64-encoded RSA key file.
    /// If an error occurs (i.e., an incorrect key), it logs the exception and returns an empty string.
    /// </remarks>
    public static string Decrypt(string message, string? privateKeyPath = null)
    {
        try
        {
            if (string.IsNullOrEmpty(privateKeyPath))
            {
                string email = ConfigManager.Get("SMTP_USER");
                privateKeyPath = Path.Combine(KeyManager.DefaultDirectory, $"{email}_private.pem");

                if (!File.Exists(privateKeyPath))
                {
                    Log.Error("Private key does not exist. Set an email address in the .env file and generate a key pair, or specify a key file (-k, --key).");
                    return String.Empty;
                }
            }
            else if (!File.Exists(privateKeyPath))
            {
                Log.Error($"Private key {privateKeyPath} does not exist.");
                return String.Empty;
            }

            byte[] privateKeyBytes = Convert.FromBase64String(File.ReadAllText(privateKeyPath));
            byte[] encryptedMessageBytes = Convert.FromBase64String(message);
            
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

            byte[] decryptedMessageBytes = rsa.Decrypt(encryptedMessageBytes, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decryptedMessageBytes);
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
            return string.Empty;
        }
    }
}