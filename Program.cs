using Microsoft.Extensions.DependencyInjection;
using MyCrypt.Commands.Encrypt;
using MyCrypt.Infrastructure;
using Spectre.Console.Cli;

var services = new ServiceCollection()
                    .AddMyCrypt();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.AddCommand<EncryptCommand>("encrypt");
});

return app.Run(args);