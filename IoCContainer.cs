using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestClient.Services;
using System;
using System.IO;

namespace RestClient
{
    public static class IoCContainer
    {
        private static IConfiguration _configuration
            = BuildConfiguration();

        private static IServiceProvider _serviceProvider
            = BuildServiceProvider();

        public static T Resolve<T>()
            => _serviceProvider.GetRequiredService<T>();

        private static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddHttpClient();
            services.AddScoped<IRestService, RestService>();

            return services.BuildServiceProvider();
        }
    }
}
