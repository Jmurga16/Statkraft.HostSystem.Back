using System;
using System.Threading.Tasks;

namespace Stakraft.HostSystem.Service.Service
{
    public interface IScheduleService
    {
        Task CrearArchivoBanco(string bei, string pais);
        Task TransferirArchivosSFTP();
        Task LeerRespuestaOutSFTP(String prefixOut, string prefixRecibido, string prefixInvalido, string prefixAutorizado);
    }
}
