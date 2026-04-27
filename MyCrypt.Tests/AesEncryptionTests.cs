using Microsoft.Extensions.DependencyInjection;
using MyCrypt.Commands;
using MyCrypt.Factories;
using MyCrypt.Infrastructure;
using MyCrypt.Interfaces;
using Spectre.Console.Cli.Testing;
using System.Security.Cryptography;

namespace MyCrypt.Tests
{
    public class AesEncryptionTests : IAsyncLifetime
    {
        string _dir;

        string Input => Path.Combine(_dir, "input.txt");
        string OutputDefaultFilename => Path.Combine(_dir, "output");

        string TextPassword = "MyTestPassword";



        // Set working folder for ruuning the tests
        public Task InitializeAsync()
        {
            _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_dir);

            File.WriteAllText(Input, "Hello world!");

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            Directory.Delete(_dir, true);
            return Task.CompletedTask;
        }

        [Fact]
        public void Encrypt_With_No_Explicit_Key() 
        {
            //Arrange
            string fileExtraId = "NoKey";

            var services = new ServiceCollection().AddMyCrypt();
            var registrar = new TypeRegistrar(services);

            var app = new CommandAppTester(registrar);
            app.Configure(config =>
            {
                config.AddCommand<EncryptCommand>("encrypt");
            });

            //Act
            var result = 
            app.Run(new[]
            {
                "encrypt",
                Input,
                "-o", $"{OutputDefaultFilename}{fileExtraId}"
            });

            //Assert
            Assert.Equal(0, result.ExitCode);
        }

        [Fact]
        public void Encrypt_Then_Decrypt_With_Text_Password()
        {
            //Arrange
            string fileExtraId = "TextPassword";

            var services = new ServiceCollection().AddMyCrypt();
            var registrar = new TypeRegistrar(services);

            var app = new CommandAppTester(registrar);
            app.Configure(config =>
            {
                config.AddCommand<EncryptCommand>("encrypt");
                config.AddCommand<DecryptCommand>("decrypt");
            });

            //Act
            var resultEncrypt =
            app.Run(new[]
            {
                "encrypt",
                Input,
                "-o", $"{OutputDefaultFilename}{fileExtraId}",
                "-k", TextPassword
            });

            var resultDecrypt =
            app.Run(new[]
            {
                "decrypt",
                $"{OutputDefaultFilename}{fileExtraId}.myc",
                TextPassword,
                "-o", $"{OutputDefaultFilename}{fileExtraId}",
            });

            //Assert
            Assert.Equal(0, resultEncrypt.ExitCode);
            Assert.Equal(0, resultDecrypt.ExitCode);
            Assert.Equal(File.ReadAllBytes(Input), File.ReadAllBytes($"{OutputDefaultFilename}{fileExtraId}.txt"));
        }
    }
}
