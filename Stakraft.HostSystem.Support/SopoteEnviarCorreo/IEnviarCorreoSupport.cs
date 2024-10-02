using Stakraft.HostSystem.Support.SoporteDto;

namespace Stakraft.HostSystem.Support.SopoteEnviarCorreo
{
    public interface IEnviarCorreoSupport
    {
        Task enviarCorreo(CorreoInfo correoInfo);
    }
}
