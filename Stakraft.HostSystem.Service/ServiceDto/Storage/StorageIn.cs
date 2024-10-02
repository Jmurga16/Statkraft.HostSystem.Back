using Stakraft.HostSystem.Support.SoporteJsonConverter;
using System.Text.Json.Serialization;

namespace Stakraft.HostSystem.Service.ServiceDto.Storage
{
    public class StorageIn
    {
        [JsonConverter(typeof(CifradorConverter))]
        public string StringConnection { get; set; }
        [JsonConverter(typeof(CifradorConverter))]
        public string Container { get; set; }
        public string Usuario { get; set; }
        public int TipoStorage { get; set; }
    }
}
