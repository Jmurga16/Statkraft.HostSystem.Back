using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
