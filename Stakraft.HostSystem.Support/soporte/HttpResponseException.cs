using System.Runtime.Serialization;

namespace Stakraft.HostSystem.Support.soporte
{
    [Serializable]
    public class HttpResponseException : Exception
    {
        public int Status { get; set; } = 500;

        public object Value { get; set; }

        protected HttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
