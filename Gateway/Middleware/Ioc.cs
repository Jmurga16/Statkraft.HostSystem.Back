using Microsoft.Extensions.DependencyInjection;
//using Serilog;

namespace Gateway.Middleware
{
    public static class Ioc
    {
        public static IServiceCollection AddDependency(this IServiceCollection services)
        {
            //services.AddSingleton(Log.Logger);

            return services;
        }
    }
}
