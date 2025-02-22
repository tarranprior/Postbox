using System.Security.Cryptography;
using Postbox.Configuration;
using Postbox.Utilities;
using Serilog;

namespace Postbox.KeyManagement;

/// <summary>
/// Provides functionality for key management.
/// </summary>
public static class KeyManager
{
    /// <summary>
    /// The default directory where key files are written.
    /// </summary>
    public static readonly string DefaultDirectory =
    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Postbox", "Keys");

    /// <summary>
    /// Generates an RSA key pair and saves the private and public key files to the default directory.
    /// </summary>
    /// <param name="bits">The key size in bits (must be 2048, 3072, or 4096). This value will never be null.</param>
    /// <remarks>
    /// If an unsupported key size is provided, the default size of 2048 bits is set.
    /// The keys are written as Base64 encoded PEM files in the user's home directory.
    /// </remarks>
    public static async Task GenerateKeys(int bits)
    {
        string email = ConfigManager.Get("SMTP_USER");
        int[] keySizes = [2048, 3072, 4096];

        if (string.IsNullOrEmpty(email) || email == "YOUR_EMAIL_ADDRESS")
        {
            Log.Error("Please update the `.env` file with your email address before generating a key pair.");
            return;
        }
        else if (!Validation.IsEmail(email))
        {
            Log.Error("The email address from the `.env` is not valid. Please correct it and try again.");
            return;
        }

        if (!keySizes.Contains(bits))
        {
            Log.Warning($"Postbox doesn't support `{bits}` as a key size. Defaulting to 2048 bits.");
            bits = 2048;
        }

        using var rsa = RSA.Create(bits);

        if (!Directory.Exists(DefaultDirectory))
        {
            Directory.CreateDirectory(DefaultDirectory);
        }

        string privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
        string privateKeyPath = Path.Combine(DefaultDirectory, $"{email.ToLower()}_private.pem");
        string publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
        string publicKeyPath = Path.Combine(DefaultDirectory, $"{email.ToLower()}_public.pem");

        if (File.Exists(privateKeyPath) || File.Exists(publicKeyPath))
        {
            Log.Error($"Keys for `{email}` already exist!");
            return;
        }

        try
        {
            await File.WriteAllTextAsync((privateKeyPath), privateKey);
            await File.WriteAllTextAsync((publicKeyPath), publicKey);
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
            return;
        }

        Log.Information($"Keys successfully written to `{DefaultDirectory}`.");
    }

    /// <summary>
    /// Gets the file path of a public key from the default directory.
    /// </summary>
    /// <returns>
    /// The file path of the public key if it exists; otherwise, an empty string.
    /// </returns>
    public static string GetPublicKey()
    {
        string email = ConfigManager.Get("SMTP_USER");
        string publicKeyPath = Path.Combine(DefaultDirectory, $"{email.ToLower()}_public.pem");

        if (File.Exists(publicKeyPath))
        {
            return publicKeyPath;
        }
        return string.Empty;
    }

    /// <summary>
    /// Imports a public key file and associates it with the specified email address.
    /// </summary>
    /// <param name="key">The file path to the public key being imported.</param>
    /// <param name="email">The email address to associate with the imported key.</param>
    /// <remarks>
    /// The imported key will be stored in the user's key directory (`Postbox/Keys`) as `{email}_public.pem`.
    /// If a key already exists for the given email, the user will be prompted before overwriting.
    /// </remarks>
    public static async Task ImportKey(string key, string email)
    {
        string destination = Path.Combine(KeyManager.DefaultDirectory, $"{email.ToLower()}_public.pem");

        if (File.Exists(destination))
        {
            Log.Warning($"A public key already exists for `{email}`.");
            Console.Write("Overwrite existing key? (y/n): ");

            string? response = Console.ReadLine()?.Trim().ToLower();

            if (response != "y")
            {
                return;
            }
        }

        await Task.Run(() => File.Copy(key, destination, true));
        Log.Information($"Key successfully imported for `{email}`.");
    }
}