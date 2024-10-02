using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.RepositoryDto.Usuario;

namespace Stakraft.HostSystem.Repository.Repository
{
    public interface IUsuarioRepository
    {
        List<UsuarioRepositoryDto> ListarUsuarios(bool activos);

        void CrearUsuario(Usuarios usuario);

        void EditarUsuario(Usuarios usuario);

        Usuarios ObtenerUsuario(int idUsuario);
    }
}
