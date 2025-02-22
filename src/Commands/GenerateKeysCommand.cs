using System.CommandLine;
using Postbox.KeyManagement;
using Serilog;

namespace Postbox.Commands;

public class GenerateKeysCommand
{
    private const int DefaultKeySize = 2048;

    public static Command Create()
    {
        var bitsArgument = new Argument<int?>("Specify a key size (2048, 3072, 4096). Default is 2048.") { Arity = ArgumentArity.ZeroOrOne };
        var bitsOption = new Option<int?>(["-b", "--bits"], () => DefaultKeySize, "Specify a key size (2048, 3072, 4096). Default is 2048.") { Arity = ArgumentArity.ZeroOrOne };

        var command = new Command("generate-keys", "Generates a new key pair.")
        {
            bitsArgument, bitsOption
        };

        command.SetHandler(async (int? bitsArg, int? bitsOpt) =>
        {
            int? bits = bitsArg ?? bitsOpt;

            if (bits == null)
            {
                Log.Error("Please specify a valid key size (2048, 3072, 4096).");
                return;
            }

            await KeyManager.GenerateKeys(bits.Value);
        }, bitsArgument, bitsOption);

        return command;
    }
}