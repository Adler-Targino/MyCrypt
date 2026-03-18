using MyCrypt.Interfaces;
using MyCrypt.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text;

namespace MyCrypt.Commands
{
    [Description("Encrypts a file using a cryptographic key.")]
    internal class EncryptCommand : Command<EncryptCommand.Settings>
    {
        private readonly IAesUtilService _aesUtilService;
        public EncryptCommand(IAesUtilService aesUtilService)
        {
            _aesUtilService = aesUtilService;
        }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<FILE>")]
            [Description("Content to be encrypted.")]
            public required FileInfo Input { get; init; }
            
            [CommandOption("-o|--output <PATH>")]
            [Description("Output path for encrypted file.")]
            public string? Output { get; init; }

            [CommandOption("-k|--key [STRING]")]
            [Description("Key used to encrypt the file. (Default: New random 32 bytes key)")]
            public required FlagValue<string> Key { get; init; }

            [CommandOption("-d|--delete")]
            [Description("Deletes the original file after encryption.")]
            public bool DeleteOriginal { get; init; }
        }

        public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            byte[] key;
            if (!settings.Key.IsSet)
            {
                key = _aesUtilService.GenerateRandomKey();
            }
            else
            {
                key = _aesUtilService.ParseKey(settings.Key.Value);
            }

            if (!settings.Input.Exists)
            {
                throw new FileNotFoundException($"Input file not found: {settings.Input.FullName}");
            }

            string inputExtension = settings.Input.Extension;
            string outputFilename = PathResolverService.ResolveEncryptedFileName(settings.Input, settings.Output);
            
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
                               _aesUtilService.EncryptFile(input, output, key, inputExtension);
                           });

                input.Close();
                output.Close();
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
