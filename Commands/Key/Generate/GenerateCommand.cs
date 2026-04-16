using MyCrypt.Helpers;
using MyCrypt.Interfaces;
using MyCrypt.Models;
using MyCrypt.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace MyCrypt.Commands
{
    internal class GenerateCommand : Command<GenerateCommand.Settings>
    {
        private readonly IEncryptionServiceFactory _factory;
        public GenerateCommand(IEncryptionServiceFactory factory)
        {
            _factory = factory;
        }

        public class Settings : CommandSettings
        {
            [CommandOption("-t|--type <STRING>")]
            [Description("Key pair type to be created.")]
            [DefaultValue("AES")]
            public string? KeyType { get; init; } = "AES";

            [CommandOption("-e|--export [PATH]")]
            [Description("Exports the generated key into a file.")]
            public FlagValue<string> Export { get; init; } = new FlagValue<string>();
        }

        public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            byte[] key;

            if (!Enum.TryParse<EncryptionType>(settings.KeyType, true, out var algorithm))
            {
                AnsiConsole.MarkupLine($"Unsupported type: [yellow]'{settings.KeyType}'[/]");
                return 0;
            }

            IEncryptionService _encryptionService = _factory.Create(algorithm);
            key = _encryptionService.GenerateRandomKey();

            AnsiConsole.MarkupLine($"{settings.KeyType.ToUpperInvariant()} Key successfully generated: " +
                $"[yellow]{Convert.ToBase64String(key)}[/]");

            if (settings.Export.IsSet)
            {
                string path = PathResolvingHelper.ResolveKeyFileName(
                    !string.IsNullOrWhiteSpace(settings.Export.Value) ? settings.Export.Value :
                    $"mycrypt-key-{settings.KeyType.ToLowerInvariant()}-{DateTime.Now:yyyyMMdd-HHmmss}");

                EncryptionKeyFile.ExportKey(key, algorithm, path);

                AnsiConsole.MarkupLine($"Key exported to file: " +
                $"[yellow]{path}[/]");
            }


            return 0;
        }
    }
}
