using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.RepositoryDto.Perfill;

namespace Stakraft.HostSystem.Repository.Repository
{
    public interface IPerfilRepository
    {
        List<PerfilRepositoryDto> ListarPerfiles(bool activos);

        void CrearPerfil(Perfiles perfil);

        void EditarPerfil(Perfiles perfil);

        Perfiles ObtenerPerfil(int perfilId);
    }
}
