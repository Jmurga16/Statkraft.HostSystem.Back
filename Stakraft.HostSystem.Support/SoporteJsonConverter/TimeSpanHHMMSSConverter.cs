using Stakraft.HostSystem.Support.soporte;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Stakraft.HostSystem.Support.SoporteJsonConverter
{
    public class TimeSpanHhmmssConverter : JsonConverter<TimeSpan?>
    {
        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return TimeSpan.ParseExact(reader.GetString(), @"hh\:mm", null);
            }
            catch (Exception e)
            {
                throw new StatkraftException("invalid TimeSpan value " + reader.GetString() + " excepcion : " + e.Message);
            }
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.Value.ToString(@"hh\:mm", null));

    }
}
