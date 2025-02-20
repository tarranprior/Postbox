using System.CommandLine;
using Serilog;
using Postbox.Configuration;
using Postbox.Handlers;
using Postbox.KeyManagement;
using Postbox.Utilities;

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

        var root = new RootCommand("📫 Postbox is a lightweight encryption tool which allows users to generate key pairs,\r\nexchange public keys, encrypt and decrypt messages, and communicate securely over SMTP using RSA.");

        var emailArgument = new Argument<string?>("email", "Specify an email address.") { Arity = ArgumentArity.ZeroOrOne };
        var keyArgument = new Argument<string?>("key", "Specify a public/private key.") { Arity = ArgumentArity.ZeroOrOne };
        var messageArgument = new Argument<string?>("message", "Specify a message.") { Arity = ArgumentArity.ZeroOrOne };

        var bitsOption = new Option<int>(["-b", "--bits"], () => 2048, "Specify the number of bits for a key-pair (must be 2048, 3072, or 4096). Default is 2048.");
        var emailOption = new Option<string>(["-e", "--email"], "Specify an email address.") { Arity = ArgumentArity.ZeroOrOne };
        var keyOption = new Option<string>(["-k", "--key"], "Specify a public/private key.") { Arity = ArgumentArity.ZeroOrOne };
        var messageOption = new Option<string>(["-m", "--message"], "Specify a message.") { Arity = ArgumentArity.ZeroOrOne };
        var outputOption = new Option<string>(["-o", "--output"], "Specify an output file.");

        /// <summary>
        /// Generates a new RSA key-pair with an optional specified number of bits.
        /// </summary>
        /// <param name="bits">The RSA key size (must be 2048, 3072, or 4096). Default is 2048.</param>
        var generateKeysCommand = new Command("generate-keys", "Generates a new key pair.")
        {
            bitsOption
        };
        generateKeysCommand.SetHandler(async (int bits) => await KeyManager.GenerateKeys(bits), bitsOption);

        /// <summary>
        /// Encrypts a specified message using a specified public key.
        /// </summary>
        /// <param name="messageArgument">The message to be encrypted (positional argument).</param>
        /// <param name="keyArgument">The public key used for encryption (positional argument).</param>
        /// <param name="messageOption">Optional flag (-m, --message) for specifying the message.</param>
        /// <param name="keyOption">Optional flag (-k, --key) for specifying the public key.</param>
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

        /// <summary>
        /// Decrypts a given message using the specified private key.
        /// </summary>
        /// <param name="messageArgument">The encrypted message to be decrypted (positional argument).</param>
        /// <param name="keyArgument">The private key used for decryption (positional argument).</param>
        /// <param name="messageOption">Optional flag (-m, --message) for specifying the encrypted message.</param>
        /// <param name="keyOption">Optional flag (-k, --key) for specifying the private key.</param>
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

        /// <summary>
        /// Sends a public key to a recipient.
        /// </summary>
        /// <param name="keyArgument">The key file path argument (if provided).</param>
        /// <param name="emailArgument">The recipient email argument (if provided).</param>
        /// <param name="keyOption">The key file path option (if provided).</param>
        /// <param name="emailOption">The recipient email option (if provided).</param>
        var SendKeyCommand = new Command("send-key", "Send a public key to a recipient.")
        {
            keyArgument, emailArgument, keyOption, emailOption
        };
        SendKeyCommand.SetHandler((string? keyArg, string? emailArg, string? keyOpt, string? emailOpt) =>
        {
            string? key = keyArg ?? keyOpt;
            string? email = emailArg ?? emailOpt;

            if (!string.IsNullOrEmpty(key) && string.IsNullOrEmpty(email))
            {
                if (!Validation.ValidateEmail(key))
                {
                    Log.Error("The recipient email address is not valid. Please correct it and try again.");
                    return;
                }

                email = key;
                key = KeyManager.GetPublicKey();

                if (string.IsNullOrEmpty(key))
                {
                    Log.Error("Public key does not exist. Generate a key pair with `generate-keys`, or specify a public key (-k, --key).");
                    return;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(key))
                {
                    key = KeyManager.GetPublicKey();
                    if (string.IsNullOrEmpty(key))
                    {
                        Log.Error("Public key does not exist. Generate a key pair with `generate-keys`, or specify a public key (-k, --key).");
                        return;
                    }
                }
                else
                {
                    key = Path.Combine(KeyManager.DefaultDirectory, $"{key.ToLower()}_public.pem");
                }

                if (string.IsNullOrEmpty(email))
                {
                    Log.Error("Please specify a recipient email (-e, --email).");
                    return;
                }

                if (!File.Exists(key))
                {
                    Log.Error($"Public key file for `{key}` does not exist.");
                    return;
                }

                if (!Validation.ValidateEmail(email))
                {
                    Log.Error("The recipient email address is not valid. Please correct it and try again.");
                    return;
                }
            }
            KeyExchange.SendPublicKey(key, email);
        },
        keyArgument, emailArgument, keyOption, emailOption);

        /// <summary>
        /// Handles the encryption and transmission of a message to a specified recipient.
        /// </summary>
        /// <param name="messageArg">The plaintext message provided as a positional argument.</param>
        /// <param name="emailArg">The recipient's email provided as a positional argument.</param>
        /// <param name="messageOpt">The plaintext message provided via the -m or --message option.</param>
        /// <param name="emailOpt">The recipient's email provided via the -e or --email option.</param>
        var SendMessageCommand = new Command("send-message", "Sends an encrypted message to a recipient.")
        {
            messageArgument, emailArgument, messageOption, emailOption
        };
        SendMessageCommand.SetHandler((string? messageArg, string? emailArg, string? messageOpt, string? emailOpt) =>
        {
            string? message = messageArg ?? messageOpt;
            string? email = emailArg ?? emailOpt;

            if (string.IsNullOrWhiteSpace(message))
            {
                Log.Error("Please specify a message (-m, --message) to send.");
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                Log.Error("Please specify a recipient email (-e, --email).");
                return;
            }
            if (!Validation.ValidateEmail(email))
            {
                Log.Error("The recipient email address is not valid. Please correct it and try again.");
                return;
            }

            string recipientKeyPath = Path.Combine(KeyManager.DefaultDirectory, $"{email.ToLower()}_public.pem");
            if (!File.Exists(recipientKeyPath))
            {
                Log.Error($"Public key for `{email}` does not exist. Import their public key first with `import-key`.");
                return;
            }

            string key = Path.Combine(KeyManager.DefaultDirectory, $"{email.ToLower()}");
            string encryptedMessage = MessageEncryption.Encrypt(message, key);

            KeyExchange.SendMessage(encryptedMessage, email);
        }, messageArgument, emailArgument, messageOption, emailOption);

        root.AddCommand(generateKeysCommand);
        root.AddCommand(encryptMessageCommand);
        root.AddCommand(decryptMessageCommand);
        root.AddCommand(SendKeyCommand);
        root.AddCommand(SendMessageCommand);

        await root.InvokeAsync(args);
    }
}