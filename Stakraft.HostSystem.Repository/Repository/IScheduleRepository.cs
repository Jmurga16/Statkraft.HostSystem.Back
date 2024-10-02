using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.RepositoryDto.Parametros;
using Stakraft.HostSystem.Repository.RepositoryDto.ReemplazarCaracter;

namespace Stakraft.HostSystem.Repository.Repository
{
    public interface IScheduleRepository
    {
        Archivo UpdateArchivo(HostToHostDbContext context, Archivo archivo);
        LogArchivos SaveLogArchivo(HostToHostDbContext context, LogArchivos logArchivos);
        ParametrosRepositoryDto ObtenerParametro(HostToHostDbContext context, string nombreParametro);
        List<Archivo> ListarArchivosPorEstado(HostToHostDbContext context, string banco, DateTime? fechaInicio, params int[] estado);
        TbBlobStorage ObtenerTbBlobStorage(HostToHostDbContext context, int tipoStorage, int estado);
        List<ReemplazarCaracterRepositoryDto> ListarCaracteres(HostToHostDbContext context);
        void ActualizarCorrelativo(HostToHostDbContext context, string parametros);
    }
}
