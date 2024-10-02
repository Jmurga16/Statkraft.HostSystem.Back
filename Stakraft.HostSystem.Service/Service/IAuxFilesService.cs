using Microsoft.AspNetCore.Http;
using System.Text;

namespace Stakraft.HostSystem.Service.Service
{
    public interface IAuxFilesService
    {
        string ObtenerContenidoArchivo(byte[] file);
        byte[] ObtenerFileEscribir(string contenido);
    }
}
