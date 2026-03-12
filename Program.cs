using Microsoft.Extensions.DependencyInjection;
using MyCrypt.Commands;
using MyCrypt.Infrastructure;
using Spectre.Console.Cli;

var services = new ServiceCollection()
                    .AddMyCrypt();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.AddCommand<EncryptCommand>("encrypt");
    config.AddCommand<DecryptCommand>("decrypt");

    config.AddBranch("validation", validation =>
    {
        validation.AddCommand<ValidateCommand>("validate");
        validation.AddCommand<ComputeCommand>("compute");
    });
});

return app.Run(args);