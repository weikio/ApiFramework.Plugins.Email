using Microsoft.Extensions.DependencyInjection;
using Weikio.ApiFramework.Abstractions.DependencyInjection;
using Weikio.ApiFramework.SDK;

namespace Weikio.ApiFramework.Plugins.Email
{
    public static class ServiceExtensions
    {
        public static IApiFrameworkBuilder AddEmailApi(this IApiFrameworkBuilder builder, string endpoint = null, EmailOptions configuration = null)
        {
            builder.Services.AddEmailApi(endpoint, configuration);

            return builder;
        }

        public static IServiceCollection AddEmailApi(this IServiceCollection services, string endpoint = null, EmailOptions configuration = null)
        {
            services.RegisterPlugin(endpoint, configuration);

            return services;
        }
    }
}
