using Microsoft.Extensions.DependencyInjection;
using MyCrypt.Interfaces;
using MyCrypt.Models;
using MyCrypt.Services;

namespace MyCrypt.Factories
{
    internal class EncryptionServiceFactory : IEncryptionServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public EncryptionServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEncryptionService Create(EncryptionType algorithm)
        {
            return algorithm switch
            {
                EncryptionType.Aes => _serviceProvider.GetRequiredService<AesEncryptionService>(),

                _ => throw new NotSupportedException($"Algorithm {algorithm} not supported")
            };
        }
    }
}
