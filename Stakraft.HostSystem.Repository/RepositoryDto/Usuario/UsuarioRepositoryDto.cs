﻿namespace Stakraft.HostSystem.Repository.RepositoryDto.Usuario
{
    public class UsuarioRepositoryDto
    {
        public int IdUsuario { get; set; }
        public string? UserName { get; set; }
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public int IdPerfil { get; set; }
        public string? NombrePerfil { get; set; }
        public bool Activo { get; set; }
    }
}
