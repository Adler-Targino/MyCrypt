using Microsoft.Extensions.DependencyInjection;
using MyCrypt.Factories;
using MyCrypt.Interfaces;
using MyCrypt.Services;

namespace MyCrypt.Infrastructure
{
    internal static class DependencyInjection
    {
        public static IServiceCollection AddMyCrypt(this IServiceCollection services)
        {
            // Encryption services
            services.AddSingleton<AesEncryptionService>();

            //Compression services
            services.AddSingleton<GZipCompressionService>();

            services.AddSingleton<IRngService, RngService>();
            services.AddSingleton<IEncryptionServiceFactory, EncryptionServiceFactory>();
            services.AddSingleton<ICompressionServiceFactory, CompressionServiceFactory>();
            
            return services;
        }
    }
}
