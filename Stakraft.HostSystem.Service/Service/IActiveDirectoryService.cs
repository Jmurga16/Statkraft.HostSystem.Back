using Stakraft.HostSystem.Service.ServiceDto.ActiveDirectory;
using Stakraft.HostSystem.Service.ServiceDto.Parametros;

namespace Stakraft.HostSystem.Service.Service
{
    public interface IActiveDirectoryService
    {
        List<UserAd> ListarUsuarioAd();
        string getUsuarioAd(string usuario);
        AdConfiguracionDto ObtenerConfiguracionActiveDirectory();
    }
}
