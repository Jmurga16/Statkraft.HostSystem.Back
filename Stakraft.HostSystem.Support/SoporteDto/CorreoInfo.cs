namespace Stakraft.HostSystem.Support.SoporteDto
{
    public class CorreoInfo
    {
        public string CorreoDestino { get; set; }
        public string Contenido { get; set; }
        public ArchivoAdjunto Adjunto { get; set; }
        public string Asunto { get; set; }
        public Dictionary<string, string> Parametros { get; set; }
        public CorreoConf CorreoConfiguracion { get; set; }
        public bool IsBodyHtml { get; set; }

        public class CorreoConf
        {
            public string Correo { get; set; }
            public string Clave { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
        }

        public class Property
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class ArchivoAdjunto
        {
            public byte[] File { get; set; }
            public string Nombre { get; set; }
        }
    }
}
