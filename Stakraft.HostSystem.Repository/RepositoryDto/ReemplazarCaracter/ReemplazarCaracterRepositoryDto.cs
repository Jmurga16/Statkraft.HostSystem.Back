namespace Stakraft.HostSystem.Repository.RepositoryDto.ReemplazarCaracter
{
    public class ReemplazarCaracterRepositoryDto
    {
        public int IdRCaracter { get; set; }
        public string? ValorOriginal { get; set; }
        public string? ValorReemplazo { get; set; }
        public bool Activo { get; set; }
    }
}
