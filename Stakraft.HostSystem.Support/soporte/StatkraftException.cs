using System.Runtime.Serialization;

namespace Stakraft.HostSystem.Support.soporte
{
    [Serializable]
    public class StatkraftException : Exception
    {
        public StatkraftException(string message) : base(message)
        {
        }

        public StatkraftException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StatkraftException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
