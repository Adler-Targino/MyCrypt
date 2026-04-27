using MyCrypt.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace MyCrypt.Commands
{
    [Description("Computes the cryptographic hash of a file for integrity verification.")]
    internal class ComputeHashCommand : Command<ComputeHashCommand.Settings>
    {
        private readonly IAnsiConsole _console;
        public ComputeHashCommand(IAnsiConsole console) 
        {
            _console = console;
        }

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

            _console.Status()
                       .Spinner(Spinner.Known.Dots)
                       .Start("Calculating file hash...", ctx =>
                       {
                           hash = ShaUtilService.ComputeSHA384(input);
                       });

            input.Close();

            _console.MarkupLine($"Validation hash for {settings.Input.Name}: [yellow]{hash}[/]");            

            return 0;
        }
    }
}
