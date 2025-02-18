using System.CommandLine;
using Serilog;
using Postbox.Configuration;
using Postbox.Handlers;
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
        // Serilog Configuration
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        // Postbox Configuration
        ConfigManager.LoadConfig();

        var root = new RootCommand("📫 Postbox is a lightweight command-line RSA encryption tool which allows users to generate key pairs, exchange public keys, encrypt and decrypt messages, and communicate securely over SMTP.");

        var keyArgument = new Argument<string?>("--key", "Specify a public/private key.") { Arity = ArgumentArity.ZeroOrOne };
        var messageArgument = new Argument<string?>("--message", "Specify a message.") { Arity = ArgumentArity.ZeroOrOne };

        var bitsOption = new Option<int>(["-b", "--bits"], () => 2048, "Specify the number of bits for a key-pair (must be 2048, 3072, or 4096). Default is 2048.");
        var emailOption = new Option<string>(["-e", "--email"], "Specify an email address.");
        var keyOption = new Option<string>(["-k", "--key"], "Specify a public/private key.") {Arity = ArgumentArity.ZeroOrOne};
        var messageOption = new Option<string>(["-m", "--message"], "Specify a message.") {Arity = ArgumentArity.ZeroOrOne};
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

        var encryptMessageCommand = new Command("encrypt-message", "Encrypts a message.")
        {
            messageArgument, keyArgument, messageOption, keyOption
        };
        encryptMessageCommand.SetHandler((string? messageArg, string? keyArg, string? messageOpt, string? keyOpt) =>
        {
            string? message = messageArg ?? messageOpt;
            string? key = keyArg ?? keyOpt;

            if (string.IsNullOrEmpty(message))
            {
                Log.Error("Please specify a message (-m, --message).");
                return;
            }
            else if (string.IsNullOrEmpty(key))
            {
                Log.Error("Please specify a public key (-k, --key) to encrypt the message with.");
                return;
            }
            string encryptedMessage = MessageEncryption.Encrypt(message, key);
            Log.Information($"Message: {encryptedMessage}");
        },
        messageArgument, keyArgument, messageOption, keyOption);

        var decryptMessageCommand = new Command("decrypt-message", "Decrypts a message.")
        {
            messageArgument, keyArgument, messageOption, keyOption
        };
        decryptMessageCommand.SetHandler((string? messageArg, string? keyArg, string? messageOpt, string? keyOpt) =>
        {
            string? message = messageArg ?? messageOpt;
            string? key = keyArg ?? keyOpt;

            if (string.IsNullOrEmpty(message))
            {
                Log.Error("Please specify a message (-m, --message) to decrypt.");
                return;
            }
            string decryptedMessage = MessageDecryption.Decrypt(message, key);
            Log.Information($"Message: {decryptedMessage}");
        },
        messageArgument, keyArgument, messageOption, keyOption);

        root.AddCommand(generateKeysCommand);
        root.AddCommand(encryptMessageCommand);
        root.AddCommand(decryptMessageCommand);

        await root.InvokeAsync(args);
    }
}