using Microsoft.Extensions.DependencyInjection;
using MyCrypt.Commands;
using MyCrypt.Infrastructure;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Testing;

namespace MyCrypt.Tests
{
    public class AesEncryptionTests : IAsyncLifetime
    {
        string _dir;
        string Input;
        string OutputDefaultFilename;

        string TextPassword = "MyTestPassword";


        // Set working folder for ruuning the tests
        public Task InitializeAsync()
        {
            _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Input = Path.Combine(_dir, "input.txt");
            OutputDefaultFilename = Path.Combine(_dir, "output");
            
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

        [Fact] //Also adds the no hmac validation option for testing
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
                "-k", TextPassword,
                "-m", "None"
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

        [Fact] //Adds the GZip compression option for testing
        public void Encrypt_Then_Decrypt_With_Generated_Key()
        {
            //Arrange
            string fileExtraId = "GeneratedKey";

            var services = new ServiceCollection().AddMyCrypt();
            var registrar = new TypeRegistrar(services);

            var app = new CommandAppTester(registrar);
            app.Configure(config =>
            {
                config.AddCommand<EncryptCommand>("encrypt");
                config.AddCommand<DecryptCommand>("decrypt");

                config.AddBranch<CommandSettings>("key", key =>
                {
                    key.AddCommand<GenerateCommand>("generate");
                });
            });

            //Act
            var resultGenerate =
            app.Run(new[]
            {
                "key", "generate",
                "-e", $"{OutputDefaultFilename}{fileExtraId}"
            });

            var resultEncrypt =
            app.Run(new[]
            {
                "encrypt",
                Input,
                "-o", $"{OutputDefaultFilename}{fileExtraId}",
                "-k", $"{OutputDefaultFilename}{fileExtraId}.myk",
                "-c", "GZip"
            });

            var resultDecrypt =
            app.Run(new[]
            {
                "decrypt",
                $"{OutputDefaultFilename}{fileExtraId}.myc",
                $"{OutputDefaultFilename}{fileExtraId}.myk",
                "-o", $"{OutputDefaultFilename}{fileExtraId}",
            });

            //Assert
            Assert.Equal(0, resultGenerate.ExitCode);
            Assert.Equal(0, resultEncrypt.ExitCode);
            Assert.Equal(0, resultDecrypt.ExitCode);
            Assert.Equal(File.ReadAllBytes(Input), File.ReadAllBytes($"{OutputDefaultFilename}{fileExtraId}.txt"));
        }

        [Fact]
        public void Decrypt_With_Wrong_Password()
        {
            //Arrange
            string fileExtraId = "WrongPassword";

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
                "-k", TextPassword,
            });

            var resultDecrypt =
            app.Run(new[]
            {
                "decrypt",
                $"{OutputDefaultFilename}{fileExtraId}.myc",
                "NotMyPassword",
                "-o", $"{OutputDefaultFilename}{fileExtraId}",
            });

            //Assert
            Assert.Equal(0, resultEncrypt.ExitCode);
            Assert.NotEqual(0, resultDecrypt.ExitCode);
            Assert.Contains("Decryption failed", resultDecrypt.Output);
        }
    }
}
