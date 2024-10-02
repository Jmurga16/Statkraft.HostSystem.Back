using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.RepositoryDto.ReemplazarCaracter;

namespace Stakraft.HostSystem.Repository.Repository
{
    public interface IReemplazaCaracterRepository
    {
        List<ReemplazarCaracterRepositoryDto> ListarCaracteres(bool activos);

        void CrearReemplazoCaracter(ReemplazoCaracter rCaracter);

        void EditarReemplazoCaracter(ReemplazoCaracter rCaracter);

        ReemplazoCaracter ObtenerReemplazoCaracter(int rCaracterId);
    }
}
