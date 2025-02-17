using System.CommandLine;

namespace Postbox;

class Program
{
    static void Main(string[] args)
    {
        var root = new RootCommand("📫 Postbox is a lightweight command-line RSA encryption tool which allows users to generate key pairs, exchange public keys, encrypt and decrypt messages, and communicate securely over SMTP.");

        var messageOption = new Option<string>(["-m", "--message"], "Specify a message.");
        var fileOption = new Option<string>(["-f", "--file"], "Specify a file.");
        var keyOption = new Option<string>(["-k", "--key"], "Specify a public/private key.");
        var outputOption = new Option<string>(["-o", "--output"], "Specify an output file.");
        var emailOption = new Option<string>(["-e", "--email"], "Specify an email address.");

        root.Invoke(args);
    }
}