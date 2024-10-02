namespace Stakraft.HostSystem.Service.ServiceDto.Seguridad
{
    public class LoginOut
    {
        public LoginUsuarioOut Usuario { get; set; }
        public TokenDto Token { get; set; }

        public class LoginUsuarioOut
        {
            public int IdUsuario { get; set; }
            public int? IdPerfil { get; set; }
            public string Usuario { get; set; }
            public string Perfil { get; set; }
            public string FechaLogin { get; set; }
        }
    }
}
