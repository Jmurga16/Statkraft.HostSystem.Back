using Hangfire;
using Hangfire.MemoryStorage;

using Gateway.Middleware;
using Gateway.Services;
using Microsoft.EntityFrameworkCore;
using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Support.soporte;
using Serilog.Events;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;


// Add services to the container.

builder.Services.AddDbContext<HostToHostDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);


//Agregar CORS
builder.Services.ControlarCorsOrigin();
//Agregar Tokens
//builder.Services.ControlarToken(configuration);
//Controladores
builder.Services.AddControllers(options => options.Filters.Add(new HttpResponseExceptionFilter()));
//Excepcion de Peticion
builder.Services.ControlarExcepcionPeticion();

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = null;
});

//Inyeccion de Dependencia 
builder.Services.AddDependency();

//Configuracion HangFire
builder.Services.AddHangfire(config =>
                 config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                 .UseSimpleAssemblyNameTypeSerializer()
                 .UseDefaultTypeSerializer()
                 .UseMemoryStorage()
                 );
builder.Services.AddHangfireServer();

//Documentacion Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Api Host to Host 2024" });
});


//Configuracion de LOG
void SetupSerilog(IConfiguration configurationApp)
{
    var path = configurationApp["Serilog:PathLog"];
    var pathErrorFinal = Path.Combine(path + "host-log-error.txt");
    var pathDebuFinal = Path.Combine(path + "host-log-debug.txt");
    Console.WriteLine("SetupSerilog" + Directory.GetCurrentDirectory());
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json").Build();

    Log.Logger = new LoggerConfiguration().ReadFrom
        .Configuration(configuration)
        .Enrich.FromLogContext()
        .MinimumLevel.Debug()
        .WriteTo.Console()
      //.WriteTo.RollingFile(pathErrorFinal, LogEventLevel.Warning, retainedFileCountLimit: 15)
      //.WriteTo.RollingFile(pathDebuFinal, LogEventLevel.Debug, retainedFileCountLimit: 1)
      .CreateLogger();
    Log.Debug("Successfully setup Serilog");
}


var app = builder.Build();

app.UseHangfireDashboard();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
