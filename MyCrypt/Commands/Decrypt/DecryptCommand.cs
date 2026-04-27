using MyCrypt.Helpers;
using MyCrypt.Interfaces;
using MyCrypt.Models;
using MyCrypt.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Security.Cryptography;

namespace MyCrypt.Commands
{
    [Description("Decrypts a previously encrypted .myc file.")]
    internal class DecryptCommand : Command<DecryptCommand.Settings>
    {
        private readonly IAnsiConsole _console;
        private readonly IEncryptionServiceFactory _factory;
        public DecryptCommand(IEncryptionServiceFactory factory, IAnsiConsole console)
        {
            _factory = factory;
            _console = console;
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
            if (!settings.Input.Exists)
            {
                throw new FileNotFoundException($"Input file not found: {settings.Input.FullName}");
            }

            using var input = settings.Input.OpenRead();

            EncryptedFileHeader fileHeader = EncryptedFileHeader.ReadHeaderFromStream(input);
            IEncryptionService _encryptionService = _factory.Create(fileHeader.Encryption);

            byte[] key;
            if (File.Exists(settings.Key))
            {
                key = EncryptionKeyFile.ImportKey(settings.Key, fileHeader.Encryption);
            }
            else
            {
                key = _encryptionService.ParseKey(settings.Key);
            }


            string outputFilename = PathResolvingHelper.ResolveDecryptedtFileName(settings.Input, settings.Output);

            if (File.Exists(outputFilename))
            {
                if (!_console.Confirm($"File [yellow]{Path.GetFileName(outputFilename)}[/] already exists. Do you want to [red]Overwrite[/]?"))
                {
                    return 1;
                }
            }

            using var output = File.Create(outputFilename);

            try
            {
                _console.Status()
                           .Spinner(Spinner.Known.Dots)
                           .Start("Decrypting file...", ctx =>
                           {
                               _encryptionService.DecryptFile(input, output, key, fileHeader);
                           });
            }
            catch (Exception ex)
            {
                _console.MarkupLine($"[red]Decryption failed.[/] {ex.Message}");

                output.Dispose();
                if (File.Exists(outputFilename))
                    File.Delete(outputFilename);

                return 1;
            }
            
            if (settings.DeleteOriginal)
            {
                input.Dispose();
                settings.Input.Delete();
            }

            _console.WriteLine("File Decrypted successfully");
            return 0;
        }
    }
}
