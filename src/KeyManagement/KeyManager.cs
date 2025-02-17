using System.Security.Cryptography;
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
    private static readonly string DefaultDirectory =
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
        int[] keySizes = [2048, 3072, 4096];

        if (!keySizes.Contains(bits))
        {
            Log.Warning($"Postbox doesn't support {bits} as a key size. Defaulting to 2048 bits.");
            bits = 2048;
        }

        using var rsa = RSA.Create(bits);

        if (!Directory.Exists(DefaultDirectory))
        {
            Directory.CreateDirectory(DefaultDirectory);
        }
        string privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
        string publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());

        await File.WriteAllTextAsync(Path.Combine(DefaultDirectory, "private_key.pem"), privateKey);
        await File.WriteAllTextAsync(Path.Combine(DefaultDirectory, "public_key.pem"), publicKey);

        Log.Information($"Keys written to {DefaultDirectory}");
    }
}