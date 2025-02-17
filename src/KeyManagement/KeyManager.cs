using System.Security.Cryptography;
using Serilog;

namespace Postbox.KeyManagement;

public static class KeyManager
{
    private static readonly string DefaultDirectory =
    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Postbox", "Keys");

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