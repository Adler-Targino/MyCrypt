using Microsoft.Extensions.DependencyInjection;
using MyCrypt.Commands.Encrypt;
using MyCrypt.Interfaces;
using MyCrypt.Services;

namespace MyCrypt.Infrastructure
{
    internal static class DependencyInjection
    {
        public static IServiceCollection AddMyCrypt(this IServiceCollection services)
        {
            // services
            services.AddSingleton<IAesUtilService, AesUtilService>();
            services.AddSingleton<IRngService, RngService>();

            // commands
            services.AddSingleton<EncryptCommand>();

            return services;
        }
    }
}
