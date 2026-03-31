using Microsoft.Extensions.DependencyInjection;
using MyCrypt.Commands;
using MyCrypt.Interfaces;
using MyCrypt.Services;

namespace MyCrypt.Infrastructure
{
    internal static class DependencyInjection
    {
        public static IServiceCollection AddMyCrypt(this IServiceCollection services)
        {
            // services
            services.AddSingleton<IEncryptionService, AesUtilService>();
            services.AddSingleton<IRngService, RngService>();

            return services;
        }
    }
}
