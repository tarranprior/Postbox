using System.Security.Cryptography;
using System.Text;
using Postbox.KeyManagement;
using Serilog;

namespace Postbox.Handlers;

/// <summary>
/// Provides functionality for encrypting messages using RSA encryption.
/// </summary>
public static class MessageEncryption
{
    /// <summary>
    /// Encrypts a specified message using a public key file.
    /// </summary>
    /// <param name="message">The plaintext message to encrypt.</param>
    /// <param name="publicKeyPath">The file path to the public key file.</param>
    /// <returns>
    /// A Base64-encoded string of the encrypted message, or an empty string if encryption fails.
    /// </returns>
    /// <remarks>
    /// This method uses RSA encryption with OAEP padding and SHA-256 hashing.
    /// The specified public key must be a valid Base64-encoded RSA public key.
    /// If an error occurs (i.e., invalid key format), it logs the exception and returns an empty string.
    /// </remarks>
    public static string Encrypt(string message, string publicKeyPath)
    {
        try
        {
            publicKeyPath = Path.Combine(KeyManager.DefaultDirectory, $"{publicKeyPath}_public.pem");
            if (!File.Exists(publicKeyPath))
            {
                Log.Error($"Public key {publicKeyPath} does not exist. Please import a public key associated with that email address first.");
                return string.Empty;
            }

            byte[] publicKeyBytes = Convert.FromBase64String(File.ReadAllText(publicKeyPath));

            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(publicKeyBytes, out _);

            byte[] encryptedMessageBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encryptedMessageBytes);
            
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
            return string.Empty;
        }
    }
}