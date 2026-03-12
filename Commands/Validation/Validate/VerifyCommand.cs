using MyCrypt.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text;

namespace MyCrypt.Commands
{
    [Description("Validates a file integrity using its hash.")]
    internal class VerifyCommand : Command<VerifyCommand.Settings>
    {
        public VerifyCommand() { }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<FILE>")]
            [Description("Content to be validated.")]
            public required FileInfo Input { get; init; }

            [CommandArgument(1, "<HASH>")]
            [Description("Content to be validated.")]
            public required string Hash { get; init; }

        }
        public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            bool validationResult = false;
            var input = settings.Input.OpenRead();

            AnsiConsole.Status()
                       .Spinner(Spinner.Known.Dots)
                       .Start("Validating file...", async ctx =>
                       {
                           validationResult = ShaUtilService.ValidateSHA384(input, settings.Hash);
                       });

            input.Close();


            if (validationResult)
            {
                AnsiConsole.MarkupLine($"File is [green]valid[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"File is [red]invalid[/]");
            }

            return 0;
        }
    }
}
