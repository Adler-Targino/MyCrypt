using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MyCrypt.Commands
{
    internal class CreateCommand : Command<CreateCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<type>")]
            [Description("Key pair type to be created.")]
            public string KeyType { get; init; } = string.Empty;
        }

        public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            switch (settings.KeyType.ToLowerInvariant())
            {
                case "ecc":
                    break;
                case "rsa":
                    break;
                default:
                    Console.WriteLine($"Unsuported type '{settings.KeyType}'");
                    break;
            }
            return 0;
        }
    }
}
