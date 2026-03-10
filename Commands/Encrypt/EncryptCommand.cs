using MyCrypt.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace MyCrypt.Commands.Encrypt
{
    internal class EncryptCommand : Command<EncryptCommand.Settings>
    {
        private readonly IAesUtilService _aesUtilService;
        private static readonly int[] AesValidKeySizes = { 16, 24, 32 };
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

            [CommandOption("--key [STRING]")]
            [Description("Key used to encrypt the file. (Default: New random 32 bytes key)")]
            public required FlagValue<string> Key { get; init; }

            [CommandOption("-d|--delete")]
            [Description("Deletes the original file after encryption.")]
            public bool DeleteOriginal { get; init; }

            public override ValidationResult Validate()
            {
                if (Key.IsSet && !AesValidKeySizes.Contains(Key.Value.Length))
                {
                    return ValidationResult.Error(
                        "Invalid AES Key. The key length must be 16, 24 or 32 characters.");
                }

                return ValidationResult.Success();
            }
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
                key = Convert.FromBase64String(settings.Key.Value);
            }

            if (!settings.Input.Exists)
            {
                throw new FileNotFoundException($"Input file not found: {settings.Input.FullName}");
            }

            using var input = settings.Input.OpenRead();
            
            string outputFilename = !string.IsNullOrWhiteSpace(settings.Output) ?
                                    Path.IsPathRooted(settings.Output) ?
                                    Path.ChangeExtension(settings.Output, ".myc") :
                                    Path.ChangeExtension(
                                        Path.Combine(Directory.GetCurrentDirectory(), settings.Output), 
                                        ".myc") :
                                    Path.ChangeExtension(settings.Input.FullName, ".myc");
            
            if (File.Exists(outputFilename))
            {
                if (!AnsiConsole.Confirm($"File [yellow]{Path.GetFileName(outputFilename)}[/] already exists. Do you want to [red]Overwrite[/]?"))
                {
                    return 0;
                }
            }

            using var output = File.Create(outputFilename);
            AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .Start("Encrypting file...", async ctx =>
                {
                    _aesUtilService.EncryptFile(input, output, key);
                });

            input.Close();
            output.Close();

            if (settings.DeleteOriginal) 
            {
                settings.Input.Delete();
            }
            AnsiConsole.MarkupLine($"File encrypted sucessfully with key: [yellow]{Convert.ToBase64String(key)}[/]");
            return 0;
        }
    }
}
