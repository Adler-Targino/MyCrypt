using MyCrypt.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace MyCrypt.Commands
{
    internal class ComputeCommand : Command<ComputeCommand.Settings>
    {
        public ComputeCommand() { }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<FILE>")]
            [Description("Content to be computed.")]
            public required FileInfo Input { get; init; }
        }

        public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            string hash = string.Empty;
            var input = settings.Input.OpenRead();

            AnsiConsole.Status()
                       .Spinner(Spinner.Known.Dots)
                       .Start("Calculating file hash...", async ctx =>
                       {
                           hash = ShaUtilService.ComputeSHA384(input);
                       });

            input.Close();

            AnsiConsole.MarkupLine($"Validation hash for {settings.Input.Name}: [yellow]{hash}[/]");            

            return 0;
        }
    }
}
