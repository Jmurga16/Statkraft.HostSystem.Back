using Stakraft.HostSystem.Support.SoporteJsonConverter;
using System.Text.Json.Serialization;

namespace Stakraft.HostSystem.Service.ServiceDto.Parametros
{
    public class ParametrosDto
    {
        public int? IdParametro { get; set; }
        public string NombreParametro { get; set; }
        [JsonConverter(typeof(CifradorConverter))]
        public string ValorParametro { get; set; }
        public string Usuario { get; set; }
    }
}
