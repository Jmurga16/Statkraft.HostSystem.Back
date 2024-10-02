using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Support.SoporteEnum;

namespace Stakraft.HostSystem.Repository.Repository
{
    public interface IArchivoRepository
    {
        void GuardarArchivo(Archivo archivo);
        void GuardarLogArchivos(LogArchivos logArchivos);
        Archivo ObtenerArchivo(int idArchivo);
        void UpdateArchivo(Archivo archivo);
        List<Archivo> ListarArchivoProcesados(string texto, int? idEstado, DateTime? fechaInicio, DateTime? fechaFin, string banco);
        List<Archivo> ListarArchivosPorEstado(string banco, params int[] estado);
        List<SeleccionOpcion> ListarTipoPlantilla(string banco);
        void DisabledAllLog(int idArchivo);
        List<LogArchivos> ListarLog(string texto, int? idEstado, DateTime? fechaInicio, DateTime? fechaFin, string banco);
    }
}
