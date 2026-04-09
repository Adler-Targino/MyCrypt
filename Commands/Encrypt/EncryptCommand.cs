using MyCrypt.Helpers;
using MyCrypt.Interfaces;
using MyCrypt.Models;
using MyCrypt.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace MyCrypt.Commands
{
    [Description("Encrypts a file using a cryptographic key.")]
    internal class EncryptCommand : Command<EncryptCommand.Settings>
    {
        private readonly IEncryptionServiceFactory _factory;
        public EncryptCommand(IEncryptionServiceFactory factory)
        {
            _factory = factory;
        }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<FILE>")]
            [Description("Content to be encrypted.")]
            public required FileInfo Input { get; init; }

            [CommandOption("-o|--output <PATH>")]
            [Description("Output path for encrypted file.")]
            public string? Output { get; init; }

            [CommandOption("-a|--algorithm <ENCRYPTION>")]
            [Description("Encryption algorithm. (AES)")]
            [DefaultValue("AES")]
            public string Algorithm { get; init; } = "AES";

            [CommandOption("-k|--key [VALUE]")]
            [Description("Key used to encrypt the file.")]
            [DefaultValue("New random 32 bytes key")]
            public required FlagValue<string> Key { get; init; }

            [CommandOption("-m|--mac <MAC>")]
            [Description("Message authentication code algorithm. (HMACSHA256 | None)")]
            [DefaultValue("HMACSHA256")]
            public string Mac { get; init; } = "HMACSHA256";

            [CommandOption("-d|--delete")]
            [Description("Deletes the original file after encryption.")]
            public bool DeleteOriginal { get; init; }
        }

        public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            if (!settings.Input.Exists)
            {
                throw new FileNotFoundException($"Input file not found: {settings.Input.FullName}");
            }

            EncryptedFileHeader fileHeader = new EncryptedFileHeader();
            fileHeader.Compression = CompressionType.None;

            if (!Enum.TryParse<EncryptionType>(settings.Algorithm, true, out var algorithm))
            {
                AnsiConsole.MarkupLine($"Unsupported Encryption Algorithm: [yellow]'{settings.Algorithm}'[/]");
                return 0;
            }
            fileHeader.Encryption = algorithm;

            IEncryptionService _encryptionService = _factory.Create(algorithm);

            byte[] key;
            if (!settings.Key.IsSet)
            {
                key = _encryptionService.GenerateRandomKey();
            }
            else
            {
                if (File.Exists(settings.Key.Value))
                {
                    key = EncryptionKeyFile.ImportKey(settings.Key.Value, EncryptionType.Aes);
                }
                else
                {
                    key = _encryptionService.ParseKey(settings.Key.Value);
                }
            }

            if (!Enum.TryParse<MacType>(settings.Mac, true, out var macType))
            {
                AnsiConsole.MarkupLine($"Unsupported MAC: [yellow]'{settings.Mac}'[/]");
                return 0;
            }
            fileHeader.Mac = macType;

            string inputExtension = settings.Input.Extension;
            string outputFilename = PathResolvingHelper.ResolveEncryptedFileName(settings.Input, settings.Output);

            fileHeader.ExtensionBytes = Encoding.UTF8.GetBytes(inputExtension);

            if (File.Exists(outputFilename))
            {
                if (!AnsiConsole.Confirm($"File [yellow]{Path.GetFileName(outputFilename)}[/] already exists. Do you want to [red]Overwrite[/]?"))
                {
                    return 0;
                }
            }

            try
            {
                using var input = settings.Input.OpenRead();
                using var output = File.Create(outputFilename);

                AnsiConsole.Status()
                           .Spinner(Spinner.Known.Dots)
                           .Start("Encrypting file...", async ctx =>
                           {
                               _encryptionService.EncryptFile(input, output, key, fileHeader);
                           });

                input.Dispose();
                output.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encryption failed. {ex}");
                return 0;
            }

            if (settings.DeleteOriginal)
            {
                settings.Input.Delete();
            }

            AnsiConsole.MarkupLine($"File encrypted sucessfully with key: [yellow]" +
                $"{(settings.Key.IsSet ? settings.Key.Value : Convert.ToBase64String(key))}[/]");

            return 0;
        }
    }
}
