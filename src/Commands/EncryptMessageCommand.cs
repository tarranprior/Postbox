using System.CommandLine;
using Serilog;
using Postbox.Handlers;

namespace Postbox.Commands;

public class EncryptMessageCommand
{
    /// <summary>
    /// Encrypts a specified message using a specified public key.
    /// </summary>
    /// <param name="messageArg">The message to encrypt (positional argument).</param>
    /// <param name="keyArg">The public key (positional argument).</param>
    /// <param name="messageOpt">The message (-m, --message) to encrypt (optional flag).</param>
    /// <param name="keyOpt">The public key (-k, --key) (optional flag).</param>
    public static Command Create()
    {
        var messageArgument = new Argument<string?>("message", "Specify a plaintext message.") { Arity = ArgumentArity.ZeroOrOne };
        var keyArgument = new Argument<string?>("key", "Specify a public key.") { Arity = ArgumentArity.ZeroOrOne };
        var messageOption = new Option<string>(["-m", "--message"], "Specify a plaintext message.") { Arity = ArgumentArity.ZeroOrOne };
        var keyOption = new Option<string>(["-k", "--key"], "Specify a public key.") { Arity = ArgumentArity.ZeroOrOne };

        var command = new Command("encrypt-message", "Encrypts a message.")
        {
            messageArgument, keyArgument, messageOption, keyOption
        };

        command.SetHandler(async (string? messageArg, string? keyArg, string? messageOpt, string? keyOpt) =>
        {
            string? message = messageArg ?? messageOpt;
            string? key = keyArg ?? keyOpt;

            try
            {
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

                string encryptedMessage = await MessageEncryption.Encrypt(message, key);

                if (!string.IsNullOrEmpty(encryptedMessage))
                {
                    Log.Information($"Encrypted Message: {encryptedMessage}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Encryption failed: {ex.Message}");
            }
        }, messageArgument, keyArgument, messageOption, keyOption);

        return command;
    }
}