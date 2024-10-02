using Microsoft.Extensions.Configuration;
using Stakraft.HostSystem.Support.SoporteDto;
using System.Net;
using System.Net.Mail;
using static Stakraft.HostSystem.Support.SoporteDto.CorreoInfo;

namespace Stakraft.HostSystem.Support.SopoteEnviarCorreo.Impl
{
    public class EnviarCorreoSupport : IEnviarCorreoSupport
    {
        private readonly SmtpClient smtp = new SmtpClient();
        private readonly IConfiguration _configuration;
        public EnviarCorreoSupport(IConfiguration configuration)
        {
            _configuration = configuration;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
        }
        public async Task enviarCorreo(CorreoInfo correoInfo)
        {
            var settingsSection = _configuration.GetSection("MailConfig");
            var correoConfig = settingsSection.Get<CorreoConf>();
            var correoOrigen = correoConfig.Correo;
            var contraseniaOrigen = correoConfig.Clave;
            var host = correoConfig.Host;
            var port = correoConfig.Port;
            var isBodyHtml = correoInfo.IsBodyHtml;

            var contenido = correoInfo.Contenido;
            var asunto = correoInfo.Asunto;
            var correoDestino = correoInfo.CorreoDestino;
            try
            {
                smtp.Host = host;
                smtp.Port = port;
                smtp.Credentials = new NetworkCredential(correoOrigen, contraseniaOrigen);

                MailMessage email = new MailMessage();
                email.From = new MailAddress(correoOrigen);
                email.To.Add(new MailAddress(correoDestino));
                email.IsBodyHtml = isBodyHtml;
                email.Priority = MailPriority.Normal;
                email.Subject = asunto;
                email.Body = contenido;
                await smtp.SendMailAsync(email);
                email.Dispose();
                Console.WriteLine("Correo Electronico enviado correctamente " + correoDestino);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error envio de Correo Electronico " + correoDestino + " -> " + e.Message);
            }
        }
    }
}
