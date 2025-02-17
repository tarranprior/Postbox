using System.CommandLine;
using Serilog;
using Postbox.KeyManagement;

namespace Postbox;

class Program
{
    /// <summary>
    /// Main entry point for the Postbox application.
    /// </summary>
    /// <param name="args">The arguments passed to the application.</param>
    static async Task Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        var root = new RootCommand("📫 Postbox is a lightweight command-line RSA encryption tool which allows users to generate key pairs, exchange public keys, encrypt and decrypt messages, and communicate securely over SMTP.");

        var bitsOption = new Option<int>(["-b", "--bits"], () => 2048, "Specify the number of bits for a key-pair (must be 2048, 3072, or 4096). Default is 2048.");
        var emailOption = new Option<string>(["-e", "--email"], "Specify an email address.");
        var fileOption = new Option<string>(["-f", "--file"], "Specify a file.");
        var keyOption = new Option<string>(["-k", "--key"], "Specify a public/private key.");
        var messageOption = new Option<string>(["-m", "--message"], "Specify a message.");
        var outputOption = new Option<string>(["-o", "--output"], "Specify an output file.");

        /// <summary>
        /// Generates a new RSA key-pair with an optional specified number of bits.
        /// </summary>
        /// <param name="bits">The RSA key size (must be 2048, 3072, or 4096). Default is 2048.</param>
        var generateKeysCommand = new Command("generate-keys", "Generates a 2048-bit RSA key pair.")
        {
            bitsOption
        };
        generateKeysCommand.SetHandler(async (int bits) => await KeyManager.GenerateKeys(bits), bitsOption);

        root.AddCommand(generateKeysCommand);

        await root.InvokeAsync(args);
    }
}