using Stakraft.HostSystem.Service.ServiceDto.ActiveDirectory;
using Stakraft.HostSystem.Service.ServiceDto.Usuario;
using System.Collections.Generic;

namespace Stakraft.HostSystem.Service.Service
{
    public interface IUsuarioService
    {
        List<UsuarioDto> ListarUsuarios(bool activo);
        UsuarioDto CrearUsuario(UsuarioDto usuario);
        UsuarioDto EditarUsuario(int idUsuario, UsuarioDto usuario);
        bool CambiarEstado(int idUsuario);
        List<UserAd> ListarUsuariosAd();
    }
}
