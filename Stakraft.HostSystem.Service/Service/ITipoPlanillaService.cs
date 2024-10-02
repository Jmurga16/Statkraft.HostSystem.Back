using Stakraft.HostSystem.Service.ServiceDto.TipoPlanilla;
using System.Collections.Generic;

namespace Stakraft.HostSystem.Service.Service
{
    public interface ITipoPlanillaService
    {
        List<TipoPlanillaDto> Listar(string banco);
        TipoPlanillaDto Guardar(TipoPlanillaDto tipoPlanilla);
        TipoPlanillaDto Actualizar(TipoPlanillaDto tipoPlanilla);
        bool Estado(int idTipoPlanilla, string usuario);
    }
}
