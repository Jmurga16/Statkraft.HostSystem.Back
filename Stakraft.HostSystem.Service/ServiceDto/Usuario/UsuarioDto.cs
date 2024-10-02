namespace Stakraft.HostSystem.Service.ServiceDto.Usuario
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public int IdPerfil { get; set; }
        public string NombrePerfil { get; set; }
        public string Estado { get; set; }
        public bool Activo { get; set; }
        public string Usuario { get; set; }
    }
}
