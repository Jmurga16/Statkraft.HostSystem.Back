using Stakraft.HostSystem.Support.SoporteUtil;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Stakraft.HostSystem.Support.SoporteJsonConverter
{
    public class CifradorConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var texto = reader.GetString();
            try
            {
                var textoDecript = UtilCifrado.Encripta(texto);
                return textoDecript;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            try
            {
                var textoEncript = UtilCifrado.Desencripta(value);
                writer.WriteStringValue(textoEncript);
            }
            catch (Exception)
            {
                writer.WriteStringValue("");
            }
        }
    }
}
