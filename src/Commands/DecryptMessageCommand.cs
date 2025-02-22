using System.CommandLine;
using Serilog;
using Postbox.Handlers;

namespace Postbox.Commands;

public class DecryptMessageCommand
{
    /// <summary>
    /// Decrypts a given message using the specified private key.
    /// </summary>
    /// <param name="messageArg">The message to decrypt (positional argument).</param>
    /// <param name="keyArg">The private key (positional argument).</param>
    /// <param name="messageOpt">The message (-m, --message) to decrypt (optional flag).</param>
    /// <param name="keyOpt">The private key (-k, --key) (optional flag).</param>
    public static Command Create()
    {
        var messageArgument = new Argument<string?>("message", "Specify an encrypted message.") { Arity = ArgumentArity.ZeroOrOne };
        var keyArgument = new Argument<string?>("key", "Specify a private key.") { Arity = ArgumentArity.ZeroOrOne };
        var messageOption = new Option<string>(["-m", "--message"], "Specify an encrypted message.") { Arity = ArgumentArity.ZeroOrOne };
        var keyOption = new Option<string>(["-k", "--key"], "Specify a private key.") { Arity = ArgumentArity.ZeroOrOne };

        var command = new Command("decrypt-message", "Decrypts a message.")
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
                    Log.Error("Please specify a message (-m, --message) to decrypt.");
                    return;
                }

                string decryptedMessage = await MessageDecryption.Decrypt(message, key);

                if (!string.IsNullOrEmpty(decryptedMessage))
                {
                    Log.Information($"Decrypted Message: {decryptedMessage}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Decryption failed: {ex.Message}");
            }
        }, messageArgument, keyArgument, messageOption, keyOption);

        return command;
    }
}