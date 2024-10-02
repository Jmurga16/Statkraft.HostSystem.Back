using Stakraft.HostSystem.Service.ServiceDto.ReemplazarCaracter;
using System.Collections.Generic;

namespace Stakraft.HostSystem.Service.Service
{
    public interface IReemplazarCaracterService
    {
        List<ReemplazarCaracterDto> ListarReemplazoCaracter(bool activo);
        ReemplazarCaracterDto CrearReemplazoCaracter(ReemplazarCaracterDto rCaracter);
        ReemplazarCaracterDto EditarReemplazoCaracter(int idRCaracter, ReemplazarCaracterDto rCaracter);
        bool CambiarEstado(int idRCaracter);
    }
}
