using Microsoft.Extensions.DependencyInjection;
using MyCrypt.Interfaces;
using MyCrypt.Models;
using MyCrypt.Services;

namespace MyCrypt.Factories
{
    internal class CompressionServiceFactory : ICompressionServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CompressionServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICompressionService Create(CompressionType algorithm)
        {
            return algorithm switch
            {
                CompressionType.GZip => _serviceProvider.GetRequiredService<GZipCompressionService>(),

                _ => throw new NotSupportedException($"Compression Algorithm {algorithm} not supported")
            };
        }
    }
}
