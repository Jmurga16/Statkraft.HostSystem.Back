using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Services
{
    public static class Cors
    {
        public static IServiceCollection ControlarCorsOrigin(this IServiceCollection services)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy(
                  "MyPolicy",
                   builder => builder.WithOrigins("http://LIMWVSBT001T:8080", "http://localhost:8080",
                   "http://localhost:4200", "http://localhost:4300", "http://192.168.0.106:4200",
                   "http://limwvsbt001p", "http://localhost:50728")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials());
            });

        }
    }
}
