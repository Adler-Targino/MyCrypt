using MyCrypt.Interfaces;
using MyCrypt.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace MyCrypt.Commands
{
    internal class GenerateCommand : Command<GenerateCommand.Settings>
    {
        private readonly IEncryptionService _aesUtilService;
        public GenerateCommand(IEncryptionService aesUtilService)
        {
            _aesUtilService = aesUtilService;
        }

        public class Settings : CommandSettings
        {
            [CommandOption("-t|--type <STRING>")]
            [Description("Key pair type to be created.")]
            [DefaultValue("AES")]
            public string? KeyType { get; init; } = "aes";

            [CommandOption("-e|--export [PATH]")]
            [Description("Exports the generated key into a file.")]
            public FlagValue<string> Export { get; init; } = new FlagValue<string>();
        }

        public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            byte[] key;
            switch (settings.KeyType!.ToLowerInvariant())
            {
                case "aes":
                    key = _aesUtilService.GenerateRandomKey();
                    break;
                default:
                    AnsiConsole.MarkupLine($"Unsuported type: [yellow]'{settings.KeyType}'[/]");
                    return 0;
            }

            AnsiConsole.MarkupLine($"{settings.KeyType.ToUpperInvariant()} Key successfully generated: " +
                $"[yellow]{Convert.ToBase64String(key)}[/]");

            if (settings.Export.IsSet)
            {
                string path = PathResolverService.ResolveKeyFileName(
                    !string.IsNullOrWhiteSpace(settings.Export.Value) ? settings.Export.Value :
                    $"mycrypt-key-{settings.KeyType.ToLowerInvariant()}-{DateTime.Now:yyyyMMdd-HHmmss}");

                KeyManagementService.ExportKey(key, path);

                AnsiConsole.MarkupLine($"Key exported to file: " +
                $"[yellow]{path}[/]");
            }


            return 0;
        }
    }
}
