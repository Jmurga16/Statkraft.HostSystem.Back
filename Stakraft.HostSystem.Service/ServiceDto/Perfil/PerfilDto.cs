namespace Stakraft.HostSystem.Service.ServiceDto.Perfil
{
    public class PerfilDto
    {
        public int IdPerfil { get; set; }
        public string NombrePerfil { get; set; }
        public bool Activo { get; set; }
        public string? Estado { get; set; }
        public string? Usuario { get; set; }
    }
}
