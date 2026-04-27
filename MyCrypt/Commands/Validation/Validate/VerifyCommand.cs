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
        private readonly IAnsiConsole _console;
        public VerifyCommand(IAnsiConsole console) 
        {
            _console = console;
        }

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

            _console.Status()
                       .Spinner(Spinner.Known.Dots)
                       .Start("Validating file...", ctx =>
                       {
                           validationResult = ShaUtilService.ValidateSHA384(input, settings.Hash);
                       });

            input.Close();


            if (validationResult)
            {
                _console.MarkupLine($"File is [green]valid[/]");
                return 0;
            }
            else
            {
                _console.MarkupLine($"File is [red]invalid[/]");
                return 1;
            }
        }
    }
}
