using Stakraft.HostSystem.Service.ServiceDto.Perfil;
using System.Collections.Generic;

namespace Stakraft.HostSystem.Service.Service
{
    public interface IPerfilService
    {
        List<PerfilDto> ListarPerfiles(bool activo);
        PerfilDto CrearPerfil(PerfilDto perfil);
        PerfilDto EditarPerfil(int idPerfil, PerfilDto perfil);
        bool CambiarEstado(int idPerfil);
    }
}
