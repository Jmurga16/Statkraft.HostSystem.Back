namespace Stakraft.HostSystem.Service.ServiceDto.Archivo
{
    public class FiltroArchivoIn
    {
        public string? Texto { get; set; }
        public string? banco { get; set; }
        public int? IdEstado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
