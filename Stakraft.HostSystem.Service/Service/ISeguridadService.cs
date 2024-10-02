
using Stakraft.HostSystem.Service.ServiceDto.Seguridad;

namespace Stakraft.HostSystem.Service.Service
{
    public interface ISeguridadService
    {
        LoginOut Login(LoginIn seguridadIn);
        LoginOut Login(string usuarioAd);
        TokenDto RefreshToken(TokenDto tokenDto);
        void RevokarToken(TokenDto tokenDto);
    }
}
