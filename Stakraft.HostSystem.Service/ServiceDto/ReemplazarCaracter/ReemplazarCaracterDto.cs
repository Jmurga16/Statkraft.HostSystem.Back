namespace Stakraft.HostSystem.Service.ServiceDto.ReemplazarCaracter
{
    public class ReemplazarCaracterDto
    {
        public int IdRCaracter { get; set; }
        public string ValorOriginal { get; set; }
        public string ValorReemplazo { get; set; }
        public bool Activo { get; set; }
        public string Usuario { get; set; }
        public string? Estado { get; set; }
    }
}
