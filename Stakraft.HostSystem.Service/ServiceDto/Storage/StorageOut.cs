using System.Text.Json.Serialization;

namespace Stakraft.HostSystem.Service.ServiceDto.Storage
{
    public class StorageOut
    {
        public int Id { get; set; }
        [JsonConverter(typeof(CifradorConverter))]
        public string StringConnection { get; set; }
        [JsonConverter(typeof(CifradorConverter))]
        public string Container { get; set; }
        public int TipoStorage { get; set; }
        public string NombreTipoStorage { get; set; }
    }
}
