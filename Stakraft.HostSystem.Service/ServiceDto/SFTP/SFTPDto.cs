namespace Stakraft.HostSystem.Service.ServiceDto.SFTP
{
    public class SftpDto
    {
        public string NombreServidor { get; set; }
        public string IpServidor { get; set; }
        public string UsuarioServidor { get; set; }
        public string PasswordServidor { get; set; }
        public int PuertoServidor { get; set; }
        public string PathServidor { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public TimeSpan? HoraInicio2 { get; set; }
        public TimeSpan? HoraFin2 { get; set; }
        public int? Intervalo { get; set; }
        public int? IntervaloProceso { get; set; }
    }
}
