using MyCrypt.Helpers;
using MyCrypt.Interfaces;
using MyCrypt.Models;
using MyCrypt.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace MyCrypt.Commands
{
    [Description("Decrypts a previously encrypted .myc file.")]
    internal class DecryptCommand : Command<DecryptCommand.Settings>
    {
        private readonly IEncryptionService _encryptionService;
        public DecryptCommand(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<FILE>")]
            [Description("Content to be decrypted.")]
            public required FileInfo Input { get; init; }
            
            [CommandArgument(1, "<KEY>")]
            [Description("Key used to decrypt the file.")]
            public required string Key { get; init; }

            [CommandOption("-o|--output <PATH>")]
            [Description("Output path for decrypted file.")]
            public string? Output { get; init; }

            [CommandOption("-d|--delete")]
            [Description("Deletes the original file after decryption.")]
            public bool DeleteOriginal { get; init; }

        }
        public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            byte[] key;

            if (File.Exists(settings.Key))
            {
                key = EncryptionKeyFile.ImportKey(settings.Key, EncryptionType.Aes);
            }
            else
            {
                key = _encryptionService.ParseKey(settings.Key);
            }

            if (!settings.Input.Exists)
            {
                throw new FileNotFoundException($"Input file not found: {settings.Input.FullName}");
            }

            string outputFilename = PathResolvingHelper.ResolveDecryptedtFileName(settings.Input, settings.Output);

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
                           .Start("Decrypting file...", async ctx =>
                           {
                               _encryptionService.DecryptFile(input, output, key);
                           });

                input.Close();
                output.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption failed. {ex}");
                return 0;
            }
            
            if (settings.DeleteOriginal)
            {
                settings.Input.Delete();
            }

            Console.WriteLine("File Decrypted successfully");
            return 0;
        }
    }
}
