using Microsoft.EntityFrameworkCore;
using Stakraft.HostSystem.Repository.Entity;
using System.Linq;

namespace Stakraft.HostSystem.Repository.Repository.Impl
{
    public class SeguridadRepository : ISeguridadRepository
    {
        private readonly HostToHostDbContext _bdContext;
        public SeguridadRepository(HostToHostDbContext bdContext)
        {
            _bdContext = bdContext;
        }
        public Usuarios BuscarUsuario(string correo)
        {
            return _bdContext.Usuarios.Include(usu => usu.IdPerfilNavigation).Where(use => correo.Equals(use.UserName) && use.Activo).FirstOrDefault();
        }

        public Usuarios BuscarUsuario(string correo, string contrasenia)
        {
            return _bdContext.Usuarios
                .Include(usu => usu.IdPerfilNavigation)
                .Where(use => use.UserName.Equals(correo) && use.Activo && use.Password.Equals(contrasenia))
                .FirstOrDefault();
        }
    }
}
