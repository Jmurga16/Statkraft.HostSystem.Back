using System.Text.Json.Serialization;

namespace Stakraft.HostSystem.Service.ServiceDto.Archivo
{
    public class PlantillaProcesadaOut
    {
        public int IdArchivo { get; set; }
        public string NombreArchivoOriginal { get; set; }
        public string NombreArchivoProcesado { get; set; }
        public string MensajeError { get; set; }
        public string TipoArchivo { get; set; }
        public string Estado { get; set; }
        public string Usuario { get; set; }

        [JsonConverter(typeof(DateTimeDdmmyyyyConverter))]
        public DateTime? FechaCreacion { get; set; }

        [JsonConverter(typeof(DateTimeDdmmyyyyConverter))]
        public DateTime? FechaEnvio { get; set; }
    }
}
