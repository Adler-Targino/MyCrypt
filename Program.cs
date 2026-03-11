using Microsoft.Extensions.DependencyInjection;
using MyCrypt.Commands.Decrypt;
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
    config.AddCommand<DecryptCommand>("decrypt");
});

return app.Run(args);