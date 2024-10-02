using Microsoft.AspNetCore.Http;
using Stakraft.HostSystem.Service.ServiceDto.Archivo;
using Stakraft.HostSystem.Service.ServiceDto.Resource;
using Stakraft.HostSystem.Support.SoporteEnum;

namespace Stakraft.HostSystem.Service.Service
{
    public interface IArchivoService
    {
        Task<List<CargaArchivoOut>> GuardarArchivoOriginal(List<IFormFile> listFile, string data);
        List<PlantillaProcesadaOut> ListarBandejaPlanillas(FiltroArchivoIn plantillaProcesadaIn);
        List<PlantillaProcesadaOut> ListarBandejaReenvio(string banco);
        Task ReingresarArchivo(IFormFile file, string data);
        Task<ResourceStorage> ObtenerContenidoArchivo(int idArchivo, bool original);
        List<SeleccionOpcion> ListarTipoPlanilla(string banco);
        List<LogArchivoOut> ListarLogArchivo(FiltroArchivoIn plantillaProcesadaIn);
    }
}
