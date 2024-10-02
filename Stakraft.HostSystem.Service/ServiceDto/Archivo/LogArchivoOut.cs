using Stakraft.HostSystem.Support.SoporteJsonConverter;
using System.Text.Json.Serialization;

namespace Stakraft.HostSystem.Service.ServiceDto.Archivo
{
    public class LogArchivoOut
    {
        public string Nombre { get; set; }
        public string estado { get; set; }
        [JsonConverter(typeof(DateTimeDdmmyyyyConverter))]
        public DateTime? FechaCreacion { get; set; }
        public string Mensaje { get; set; }
    }
}
