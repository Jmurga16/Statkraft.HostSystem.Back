using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Gateway.Services
{
    public static class Token{

        public static IServiceCollection ControlarToken(this IServiceCollection services, IConfiguration configuration)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:Key"]));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    //options.UseSecurityTokenValidators = true;
                    //options.IncludeErrorDetails = true;
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //LogValidationExceptions = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidAudience = configuration["jwt:Audience"],
                        ValidIssuer = configuration["jwt:Issuer"],
                        IssuerSigningKey = symmetricSecurityKey,
                        ClockSkew = TimeSpan.FromSeconds(Convert.ToDouble(configuration["jwt:ClockSkew"])),
                    };
                });
            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            return services;
        }
    }
}
