using Gateway.Middleware;
using Gateway.Services;
using Microsoft.EntityFrameworkCore;
using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Support.soporte;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;


// Add services to the container.

builder.Services.AddDbContext<HostToHostDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);


//Agregar CORS
builder.Services.ControlarCorsOrigin();
//Agregar Tokens
//builder.Services.ControlarToken(configuration);


builder.Services.AddControllers(options => options.Filters.Add(new HttpResponseExceptionFilter()));




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Api Host to Host 2024" });
});

builder.Services.AddDependency();

var app = builder.Build();

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
