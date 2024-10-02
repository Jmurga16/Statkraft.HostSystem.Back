using Stakraft.HostSystem.Repository.Entity;

namespace Stakraft.HostSystem.Repository.Repository
{
    public interface ISeguridadRepository
    {
        Usuarios BuscarUsuario(string correo);
        Usuarios BuscarUsuario(string correo, string contrasenia);
    }
}
