using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.RepositoryDto.Usuario;
using System.Collections.Generic;
using System.Linq;

namespace Stakraft.HostSystem.Repository.Repository.Impl
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly HostToHostDbContext _bdContext;

        public UsuarioRepository(HostToHostDbContext bdContext)
        {
            _bdContext = bdContext;
        }

        public List<UsuarioRepositoryDto> ListarUsuarios(bool activos)
        {
            var query = _bdContext.Usuarios.Select(usuario => new UsuarioRepositoryDto
            {
                IdUsuario = usuario.Id,
                UserName = usuario.UserName,
                Email = usuario.Email,
                DisplayName = usuario.DisplayName,
                Activo = usuario.Activo,
                IdPerfil = (int)usuario.IdPerfil,
                NombrePerfil = usuario.IdPerfilNavigation.NombrePerfil
            });

            if (activos)
            {
                query = query.Where(usuario => usuario.Activo);
            }
            return query.ToList();
        }

        public void CrearUsuario(Usuarios usuario)
        {
            _bdContext.Usuarios.Add(usuario);
            _bdContext.SaveChanges();
        }

        public void EditarUsuario(Usuarios usuario)
        {
            _bdContext.Usuarios.Update(usuario);
            _bdContext.SaveChanges();
        }

        public Usuarios ObtenerUsuario(int idUsuario)
        {
            var usuario = _bdContext.Usuarios.Where(usuario => usuario.Id.Equals(idUsuario)).FirstOrDefault();

            return usuario;
        }
    }
}
