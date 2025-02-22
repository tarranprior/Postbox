using System.CommandLine;
using Serilog;
using Postbox.KeyManagement;
using Postbox.Utilities;

namespace Postbox.Commands;

public class SendKeyCommand
{
    /// <summary>
    /// Transmits a public key to a recipient.
    /// </summary>
    /// <param name="keyArg">The public key to send (positional argument).</param>
    /// <param name="emailArg">The recipient email address (positional argument).</param>
    /// <param name="keyOpt">The public key (-k, --key) to send (optional flag).</param>
    /// <param name="emailOpt">The recipient email address (-e, --email) (optional flag).</param>
    public static Command Create()
    {
        var keyArgument = new Argument<string?>("key", "Specify a public key.") { Arity = ArgumentArity.ZeroOrOne };
        var emailArgument = new Argument<string?>("email", "Specify a recipient email address.") { Arity = ArgumentArity.ZeroOrOne };
        var keyOption = new Option<string>(["-k", "--key"], "Specify a public key.") { Arity = ArgumentArity.ZeroOrOne };
        var emailOption = new Option<string>(["-e", "--email"], "Specify a recipient email address.") { Arity = ArgumentArity.ZeroOrOne };
        
        var command = new Command("send-key", "Send a public key to a recipient.")
        {
            keyArgument, emailArgument, keyOption, emailOption
        };

        command.SetHandler(async (string? keyArg, string? emailArg, string? keyOpt, string? emailOpt) =>
        {
            string? key = keyArg ?? keyOpt;
            string? email = emailArg ?? emailOpt;

            if (!string.IsNullOrEmpty(key) && string.IsNullOrEmpty(email))
            {
                if (!Validation.IsEmail(key))
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

                if (!Validation.IsEmail(email))
                {
                    Log.Error("The recipient email address is not valid. Please correct it and try again.");
                    return;
                }
            }
            await KeyExchange.SendPublicKey(key, email);
        },keyArgument, emailArgument, keyOption, emailOption);

        return command;
    }
}