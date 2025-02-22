using System.CommandLine;
using Serilog;
using Postbox.KeyManagement;
using Postbox.Utilities;

namespace Postbox.Commands;

public class ImportKeyCommand
{
    /// <summary>
    /// Imports a key from a key file.
    /// </summary>
    /// <param name="keyArg">The public key to import (positional argument).</param>
    /// <param name="emailArg">The recipient email address (positional argument).</param>
    /// <param name="keyOpt">The public key (-k, --key) to import (optional flag).</param>
    /// <param name="emailOpt">The recipient email address (-e, --email) (optional flag).</param>
    public static Command Create()
    {
        var keyArgument = new Argument<string?>("key", "Specify a public key to import.") { Arity = ArgumentArity.ZeroOrOne };
        var emailArgument = new Argument<string?>("email", "Specify an email address to associate with the key.") { Arity = ArgumentArity.ZeroOrOne };
        var keyOption = new Option<string>(["-k", "--key"], "Specify a public key to import.") { Arity = ArgumentArity.ZeroOrOne };
        var emailOption = new Option<string>(["-e", "--email"], "Specify an email address to associate with the key.") { Arity = ArgumentArity.ZeroOrOne };

        var command = new Command("import-key", "Imports a key from a file.")
        {
            keyArgument, emailArgument, keyOption, emailOption
        };

        command.SetHandler(async (string? keyArg, string? emailArg, string? keyOpt, string? emailOpt) =>
        {
            string? key = keyArg ?? keyOpt;
            string? email = emailArg ?? emailOpt;

            if (!File.Exists(key))
            {
                Log.Error($"The specified key file `{key}` does not exist.");
                return;
            }
            else if (string.IsNullOrEmpty(email))
            {
                Log.Error("Please specify an email (-e, --email) to associate the key with.");
                return;
            }
            if (!Validation.IsEmail(email))
            {
                Log.Error($"The provided email `{email}` is not valid.");
                return;
            }
            await KeyManager.ImportKey(key, email);
        }, keyArgument, emailArgument, keyOption, emailOption);
        return command;
    }
}