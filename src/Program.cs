using System.CommandLine;
using Serilog;
using Postbox.Configuration;
using Postbox.Commands;

namespace Postbox;

class Program
{
    /// <summary>
    /// Main entry point for the Postbox application.
    /// </summary>
    static async Task Main(string[] args)
    {
        try
        {
            // Serilog Configuration
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

            // Postbox Configuration
            await ConfigManager.LoadConfig();

            var rootCommand = new RootCommand("📫 Postbox is a lightweight encryption tool which allows users to generate key pairs,\r\nexchange public keys, encrypt and decrypt messages, and communicate securely over SMTP,\r\nusing the RSA-2048/4096 algorithm.");
            
            rootCommand.AddCommand(GenerateKeysCommand.Create());
            rootCommand.AddCommand(EncryptMessageCommand.Create());
            rootCommand.AddCommand(DecryptMessageCommand.Create());
            rootCommand.AddCommand(ImportKeyCommand.Create());
            rootCommand.AddCommand(SendKeyCommand.Create());
            rootCommand.AddCommand(SendMessageCommand.Create());

            await rootCommand.InvokeAsync(args);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}