using Stakraft.HostSystem.Support.soporte;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Stakraft.HostSystem.Support.SoporteJsonConverter
{
    public class DateTimeDdmmyyyyConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return DateTime.Parse(reader.GetString());
            }
            catch (Exception e)
            {
                throw new StatkraftException("invalid DateTime value " + reader.GetString() + " excepcion : " + e.Message);
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options) =>
                writer.WriteStringValue(value.Value.ToString("dd/MM/yyyy hh:mm", null));
    }
}
