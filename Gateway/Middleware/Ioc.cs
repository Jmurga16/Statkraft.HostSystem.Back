using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Stakraft.HostSystem.Repository.Repository;
using Stakraft.HostSystem.Repository.Repository.Impl;
using Stakraft.HostSystem.Service.Service;
using Stakraft.HostSystem.Service.Service.Impl;
using Stakraft.HostSystem.Support.SopoteEnviarCorreo;
using Stakraft.HostSystem.Support.SopoteEnviarCorreo.Impl;
using Stakraft.HostSystem.Support.Token;
using Stakraft.HostSystem.Support.Token.Impl;

namespace Gateway.Middleware
{
    public static class Ioc
    {
        public static IServiceCollection AddDependency(this IServiceCollection services)
        {
            services.AddSingleton(Log.Logger);

            services.AddTransient<IArchivoService, ArchivoService>();
            services.AddTransient<IEnviarCorreoSupport, EnviarCorreoSupport>();
            services.AddTransient<IAuxBlobStorageService, AuxBlobStorageService>();
            services.AddTransient<IAuxFilesService, AuxFilesService>();
            services.AddTransient<IUsuarioRepository, UsuarioRepository>();
            services.AddTransient<IUsuarioService, UsuarioService>();
            services.AddTransient<IActiveDirectoryService, ActiveDirectoryService>();
            services.AddTransient<IPerfilRepository, PerfilRepository>();
            services.AddTransient<IPerfilService, PerfilService>();
            services.AddTransient<IStorageRepository, StorageRepository>();
            services.AddTransient<IStorageService, StorageService>();
            services.AddTransient<IGeneralRepository, GeneralRepository>();
            services.AddTransient<IArchivoRepository, ArchivoRepository>();
            services.AddTransient<IReemplazaCaracterRepository, ReemplazaCaracterRepository>();
            services.AddTransient<IReemplazarCaracterService, ReemplazarCaracterService>();
            services.AddTransient<IParametrosRepository, ParametrosRepository>();
            services.AddTransient<IParametrosService, ParametrosService>();
            services.AddTransient<IAuxFtpService, AuxFtpService>();
            services.AddTransient<ISeguridadRepository, SeguridadRepository>();
            services.AddTransient<ISeguridadService, SeguridadService>();
            services.AddTransient<ITipoPlanillaRepository, TipoPlanillaRepository>();
            services.AddTransient<ITipoPlanillaService, TipoPlanillaService>();
            services.AddTransient<ITokenGenerador, TokenGenerador>();
            services.AddTransient<IScheduleService, ScheduleService>();
            services.AddTransient<IScheduleRepository, ScheduleRepository>();

            return services;
        }
    }
}
