using MyCrypt.Interfaces;
using MyCrypt.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace MyCrypt.Commands
{
    [Description("Decrypts a previously encrypted .myc file.")]
    internal class DecryptCommand : Command<DecryptCommand.Settings>
    {
        private readonly IAesUtilService _aesUtilService;
        public DecryptCommand(IAesUtilService aesUtilService)
        {
            _aesUtilService = aesUtilService;
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
            byte[] key = _aesUtilService.ParseKey(settings.Key);            

            if (!settings.Input.Exists)
            {
                throw new FileNotFoundException($"Input file not found: {settings.Input.FullName}");
            }

            string outputFilename = PathResolverService.ResolveDecryptedtFileName(settings.Input, settings.Output);

            if (File.Exists(outputFilename))
            {
                if (!AnsiConsole.Confirm($"File [yellow]{Path.GetFileName(outputFilename)}[/] already exists. Do you want to [red]Overwrite[/]?"))
                {
                    return 0;
                }
            }

            using var input = settings.Input.OpenRead();
            using var output = File.Create(outputFilename);

            AnsiConsole.Status()
                       .Spinner(Spinner.Known.Dots)
                       .Start("Decrypting file...", async ctx =>
                       {
                           _aesUtilService.DecryptFile(input, output, key);
                       });

            input.Close();
            output.Close();
            
            if (settings.DeleteOriginal)
            {
                settings.Input.Delete();
            }

            Console.WriteLine("File Decrypted successfully");
            return 0;
        }
    }
}
