using Microsoft.Extensions.DependencyInjection;
using MyCrypt.Commands;
using MyCrypt.Infrastructure;
using Spectre.Console.Cli;
using System.Reflection;

var services = new ServiceCollection()
                    .AddMyCrypt();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("mycrypt");
    config.SetApplicationVersion($"{Assembly.GetEntryAssembly()?.GetName().Version}");

    config.AddCommand<EncryptCommand>("encrypt");
    config.AddCommand<DecryptCommand>("decrypt");

    config.AddBranch("validation", validation =>
    {
        validation.SetDescription("Commands related to file integrity verification using hashes.");

        validation.AddCommand<VerifyCommand>("verify");
        validation.AddCommand<ComputeHashCommand>("compute-hash");
    });

    config.AddBranch("key", key =>
    {
        key.SetDescription("Commands related to key generation and validation");

    });
});

return app.Run(args);