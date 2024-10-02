using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Services
{
    public static class ExcepcionPeticion
    {
        public static IServiceCollection ControlarExcepcionPeticion(this IServiceCollection services)
        {
            return services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    string messages = "";
                    if (!context.ModelState.IsValid)
                    {
                        messages = string.Join("; ", context.ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
                    }
                    throw new ArgumentNullException(messages);
                };
            });
        }
    }
}
