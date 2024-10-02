using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Stakraft.HostSystem.Support.SoporteDto;
using Stakraft.HostSystem.Support.SoporteEnum;
using Stakraft.HostSystem.Support.SoporteUtil;
using System.Net;

namespace Stakraft.HostSystem.Support.soporte
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            Respuesta<object> respuesta = new Respuesta<object>();
            int code;
            String mensaje;
            if (ex is StatkraftException)
            {
                mensaje = ex.Message;
                code = (int)CodigosEnum.TipoRespuesta.Advertencia;
            }
            else
            {
                mensaje = Mensajes.datosIncompletos;
                code = (int)CodigosEnum.TipoRespuesta.Incorrecto;
            }

            logger.Fatal(ex, "HandleException");

            respuesta.Mensaje = mensaje;
            respuesta.MensajeDev = ex.Message + " " + ex?.InnerException?.Message;
            respuesta.Codigo = code;

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var codeStatus = mensaje.Contains("Token invalido") ? HttpStatusCode.Forbidden : HttpStatusCode.BadRequest;
            var result = JsonConvert.SerializeObject(respuesta, jsonSerializerSettings);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)codeStatus;
            return context.Response.WriteAsync(result);
        }
    }
}
