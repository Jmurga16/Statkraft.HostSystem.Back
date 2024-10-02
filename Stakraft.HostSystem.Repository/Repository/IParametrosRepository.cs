using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.RepositoryDto.Parametros;

namespace Stakraft.HostSystem.Repository.Repository
{
    public interface IParametrosRepository
    {
        ParametrosRepositoryDto ObtenerParametro(string nombreParametro);
        void ActualizarParametro(Parametros paramEntinty);
        void GuardarParametro(Parametros paramEntinty);
        Parametros ObtenerEntity(int idParam);
    }
}
