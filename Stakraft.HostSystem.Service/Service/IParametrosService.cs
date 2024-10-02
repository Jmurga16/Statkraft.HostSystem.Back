using Stakraft.HostSystem.Service.ServiceDto.Parametros;

namespace Stakraft.HostSystem.Service.Service
{
    public interface IParametrosService
    {
        ParametrosDto ObtenerParametros(string nombreParam);

        ParametrosDto ActualizarParametros(ParametrosDto dto);
    }
}
