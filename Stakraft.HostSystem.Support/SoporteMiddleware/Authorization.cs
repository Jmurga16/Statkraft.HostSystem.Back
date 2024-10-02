using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Stakraft.HostSystem.Support.SoporteMiddleware
{
    public static class Authorization
    {

        public static IServiceCollection Authorize(this IServiceCollection services)
        {

            services.AddAuthentication(x =>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(x =>
               {
                   x.RequireHttpsMetadata = false;
                   x.SaveToken = true;
                   x.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       //ValidIssuer = "0ddb4fa4-c210-4fec-9321-6f28d130cc3c", //TenantID
                       ValidIssuer = "85fd135e-c457-4e03-8144-fb5191c267eb",

                       ValidateAudience = true,
                       //ValidAudience = "6a02cf88-44f0-4c45-9072-1533326ac519", //clientID
                       ValidAudience = "e267af05-c34c-4165-bfb9-fefea8d944d8",
                   };
                   //.Audience = "6a02cf88-44f0-4c45-9072-1533326ac519"; //clientID
                   x.Audience = "e267af05-c34c-4165-bfb9-fefea8d944d8";

                   //x.Authority = "https://sts.windows.net/0ddb4fa4-c210-4fec-9321-6f28d130cc3c"; //https://sts.windows.net/TenantID
                   x.Authority = "https://sts.windows.net/85fd135e-c457-4e03-8144-fb5191c267eb";

               });

            return services;
        }
    }
}
