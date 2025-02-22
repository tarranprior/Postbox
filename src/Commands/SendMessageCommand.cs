using System.CommandLine;
using Serilog;
using Postbox.Handlers;
using Postbox.KeyManagement;
using Postbox.Utilities;

namespace Postbox.Commands;

public class SendMessageCommand
{
    /// <summary>
    /// Encrypts and transmits a message to a specified recipient over SMTP.
    /// </summary>
    /// <param name="messageArg">The plaintext message to send (positional argument).</param>
    /// <param name="emailArg">The recipient email address (positional argument).</param>
    /// <param name="messageOpt">The plaintext message (-m, --message) to send (optional flag).</param>
    /// <param name="emailOpt">The recipient email address (-e, --email) (optional flag).</param>
    public static Command Create()
    {
        var messageArgument = new Argument<string?>("message", "Specify a plaintext message.") { Arity = ArgumentArity.ZeroOrOne };
        var emailArgument = new Argument<string?>("email", "Specify a recipient email address.") { Arity = ArgumentArity.ZeroOrOne };
        var messageOption = new Option<string>(["-m", "--message"], "Specify a plaintext message.") { Arity = ArgumentArity.ZeroOrOne };
        var emailOption = new Option<string>(["-e", "--email"], "Specify a recipient email address.") { Arity = ArgumentArity.ZeroOrOne };
        
        var command = new Command("send-message", "Sends an encrypted message.")
        {
            messageArgument, emailArgument, messageOption, emailOption
        };

        command.SetHandler(async (string? messageArg, string? emailArg, string? messageOpt, string? emailOpt) =>
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

            if (!Validation.IsEmail(email))
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
            string encryptedMessage = await MessageEncryption.Encrypt(message, key);

            await KeyExchange.SendMessage(encryptedMessage, email);
        }, messageArgument, emailArgument, messageOption, emailOption);

        return command;
    }
}