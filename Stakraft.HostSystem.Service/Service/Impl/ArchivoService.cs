using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.Repository;
using Stakraft.HostSystem.Service.ServiceDto.Archivo;
using Stakraft.HostSystem.Service.ServiceDto.Resource;
using Stakraft.HostSystem.Support.soporte;
using Stakraft.HostSystem.Support.SoporteEnum;
using Stakraft.HostSystem.Support.SoporteUtil;

namespace Stakraft.HostSystem.Service.Service.Impl
{
    public class ArchivoService : IArchivoService
    {

        private readonly IAuxBlobStorageService _auxBlobStorage;
        private readonly IGeneralRepository _generalRespository;
        private readonly IArchivoRepository _archivoRepository;
        public ArchivoService(IAuxBlobStorageService auxBlobStorage, IGeneralRepository generalRespository, IArchivoRepository archivoRepository)
        {
            _auxBlobStorage = auxBlobStorage;
            _generalRespository = generalRespository;
            _archivoRepository = archivoRepository;
        }
        public async Task<List<CargaArchivoOut>> GuardarArchivoOriginal(List<IFormFile> listFile, string data)
        {
            if (data == null)
            {
                throw new StatkraftException("object json requiered " + JsonConvert.SerializeObject(new ArchivoOriginalIn(), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            }
            if (listFile?.Count == 0)
            {
                throw new StatkraftException("No se encontro archivos a guardar.");
            }
            var unescapeData = System.Uri.UnescapeDataString(data);
            var listaImagenesGuardadas = new List<CargaArchivoOut>();
            var archivoOriginalIn = JsonConvert.DeserializeObject<ArchivoOriginalIn>(unescapeData);
            var tipoStorage = archivoOriginalIn.Banco == "BBVA" ? (int)Enums.TipoBlobStorage.OriginalBbva : (int)Enums.TipoBlobStorage.OriginalBcp;
            var tbBlobStorage = _generalRespository.obtenerTbBlobStorage(tipoStorage);
            if (tbBlobStorage == null)
            {
                throw new StatkraftException("No se encontro la configuración para el Blob Storage del archivo original.");
            }

            if (listFile?.Count > 0)
            {
                var taskList = new List<Task>();
                var fechaActual = DateTime.Now;
                for (int i = 0; i < listFile.Count; i++)
                {
                    taskList.Add(GuardarFile(listFile, archivoOriginalIn, fechaActual, listaImagenesGuardadas, tbBlobStorage, i));
                }
                await Task.WhenAll(taskList);
            }
            return listaImagenesGuardadas;

        }
        private async Task GuardarFile(List<IFormFile> listaSustentos, ArchivoOriginalIn archivoOriginalIn, DateTime fechaActual, List<CargaArchivoOut> listaImagenesGuardadas, TbBlobStorage tbBlobStorage, int i)
        {
            var file = listaSustentos[i];
            var nombreArchivo = file.FileName;
            var guardar = true;
            var cargaArchivo = new CargaArchivoOut
            {
                NombreArchivo = nombreArchivo
            };
            if (!Path.GetExtension(file.FileName).Contains("txt"))
            {
                cargaArchivo.Mensaje = nombreArchivo + " extension del archivo es invalido";
                guardar = false;
            }
            if (guardar)
            {
                var archivo = new Archivo
                {
                    BloStoIdOriginal = tbBlobStorage.Id,
                    FechaCreacion = fechaActual,
                    NombreArchivo = nombreArchivo,
                    IdTipoPlanilla = archivoOriginalIn.IdTipo,
                    IdEstado = (int)Enums.EstadoArchivo.Registrado,
                    UsuarioCreacion = archivoOriginalIn.Usuario,
                    Banco = archivoOriginalIn.Banco
                };
                _archivoRepository.GuardarArchivo(archivo);
                var logArchivo = new LogArchivos
                {
                    Activo = true,
                    FechaCreacion = fechaActual,
                    IdArchivo = archivo.Id,
                    IdEstado = (int)Enums.EstadoArchivo.Registrado,
                    Mensaje = LogMensaje.getMensajeLog(Enums.EstadoArchivo.Registrado)
                };
                _archivoRepository.GuardarLogArchivos(logArchivo);
                await _auxBlobStorage.GuardarArchivo(file, nombreArchivo, tbBlobStorage);
                cargaArchivo.Mensaje = file.FileName + " Se subió con éxito";
                cargaArchivo.Estado = (int)Enums.EstadoCargaArchivo.Correcto;
            }
            else
            {
                cargaArchivo.Estado = (int)Enums.EstadoCargaArchivo.Error;
            }
            listaImagenesGuardadas.Add(cargaArchivo);
        }
        public List<PlantillaProcesadaOut> ListarBandejaPlanillas(FiltroArchivoIn plantillaProcesadaIn)
        {
            var listaOut = _archivoRepository
                .ListarArchivoProcesados(plantillaProcesadaIn.Texto,
                plantillaProcesadaIn.IdEstado, plantillaProcesadaIn.FechaInicio,
                plantillaProcesadaIn.FechaFin, plantillaProcesadaIn.banco).Select(arc => new PlantillaProcesadaOut
                {
                    IdArchivo = arc.Id,
                    NombreArchivoOriginal = arc.NombreArchivo,
                    NombreArchivoProcesado = arc.NombreArchivoProcesado,
                    TipoArchivo = arc.IdTipoPlanillaNavigation.Nombre,
                    MensajeError = arc.MenError,
                    Estado = arc.IdEstadoNavigation.NombreEstado,
                    FechaCreacion = arc.FechaCreacion,
                    Usuario = arc.UsuarioCreacion,
                    FechaEnvio = arc.FechaModificacion
                }
           ).ToList();
            return listaOut;
        }
        public List<PlantillaProcesadaOut> ListarBandejaReenvio(string banco)
        {
            var listaOut = _archivoRepository.ListarArchivosPorEstado(banco, (int)Enums.EstadoArchivo.Erroneo, (int)Enums.EstadoArchivo.Rechazado).Select(arc => new PlantillaProcesadaOut
            {
                IdArchivo = arc.Id,
                NombreArchivoOriginal = arc.NombreArchivo,
                NombreArchivoProcesado = arc.NombreArchivoProcesado,
                MensajeError = arc.MenError,
                TipoArchivo = arc.IdTipoPlanillaNavigation.Nombre,
                Estado = arc.IdEstadoNavigation.NombreEstado,
                FechaCreacion = arc.FechaCreacion,
                Usuario = arc.UsuarioCreacion,
                FechaEnvio = arc.FechaModificacion
            }
            ).ToList();
            return listaOut;
        }
        public async Task ReingresarArchivo(IFormFile file, string data)
        {
            if (data == null)
            {
                throw new StatkraftException("object json requiered " + JsonConvert.SerializeObject(new CargaArchivoIn(), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            }
            var unescapeData = System.Uri.UnescapeDataString(data);
            var archivoReenviadoIn = JsonConvert.DeserializeObject<CargaArchivoIn>(unescapeData);
            var tipoStorage = archivoReenviadoIn.Banco == "BBVA" ? (int)Enums.TipoBlobStorage.OriginalBbva : (int)Enums.TipoBlobStorage.OriginalBcp;
            var tbBlobStorage = _generalRespository.obtenerTbBlobStorage(tipoStorage);
            if (tbBlobStorage == null)
            {
                throw new StatkraftException("No se encontro la configuración para el Blob Storage del archivo original.");
            }
            if (file == null)
            {
                throw new StatkraftException("Debe seleccionar un archivo.");
            }
            var nombreArchivo = file.FileName;
            var fechaActual = DateTime.Now;
            var archivo = _archivoRepository.ObtenerArchivo(archivoReenviadoIn.IdArchivo);
            if (archivo == null)
            {
                throw new StatkraftException("No se encontro el archivo a reenviar.");
            }

            archivo.BloStoIdOriginal = tbBlobStorage.Id;
            archivo.FechaModificacion = fechaActual;
            archivo.NombreArchivo = nombreArchivo;
            archivo.NombreArchivoProcesado = "";
            archivo.MenError = "";
            archivo.IdEstado = (int)Enums.EstadoArchivo.Reingresado;
            archivo.UsuarioCreacion = archivoReenviadoIn.Usuario;
            archivo.Banco = archivoReenviadoIn.Banco;

            var logArchivo = new LogArchivos
            {
                Activo = true,
                FechaCreacion = fechaActual,
                IdArchivo = archivo.Id,
                IdEstado = (int)Enums.EstadoArchivo.Reingresado,
                Mensaje = LogMensaje.getMensajeLog(Enums.EstadoArchivo.Reingresado)
            };
            _archivoRepository.UpdateArchivo(archivo);
            _archivoRepository.GuardarLogArchivos(logArchivo);
            await _auxBlobStorage.GuardarArchivo(file, nombreArchivo, tbBlobStorage);
        }
        public async Task<ResourceStorage> ObtenerContenidoArchivo(int idArchivo, bool original)
        {
            var archivo = _archivoRepository.ObtenerArchivo(idArchivo);
            var blodSorage = original ? archivo?.BloStoIdOriginalNavigation : archivo?.BloStoIdProcesadoNavigation;
            //TbBlobStorage blodSorage = _generalRespository.obtenerTbBlobStorage(original ? (int)Enums.TipoBlobStorage.OriginalBcp : (int)Enums.TipoBlobStorage.ProcesadoBcp);
            if (blodSorage == null)
            {
                throw new StatkraftException("No se encontro la configuración para el Blob Storage");
            }
            if (archivo == null || (!original && archivo.NombreArchivoProcesado == null))
            {
                throw new StatkraftException("No se encontro el archivo.");
            }
            var nombreArchivo = original ? archivo.NombreArchivo : archivo.NombreArchivoProcesado;
            var file = await _auxBlobStorage.ObtenerArchivo(nombreArchivo, blodSorage);
            return file;
        }
        public List<SeleccionOpcion> ListarTipoPlanilla(string banco)
        {
            return _archivoRepository.ListarTipoPlantilla(banco);
        }
        public List<LogArchivoOut> ListarLogArchivo(FiltroArchivoIn plantillaProcesadaIn)
        {
            var logArchivos = _archivoRepository
                .ListarLog(plantillaProcesadaIn.Texto, plantillaProcesadaIn.IdEstado,
                plantillaProcesadaIn.FechaInicio, plantillaProcesadaIn.FechaFin, plantillaProcesadaIn.banco);
            var logOut = logArchivos.Select(log => new LogArchivoOut
            {
                Nombre = log.IdArchivoNavigation.NombreArchivo,
                estado = Enum.GetName(typeof(Enums.EstadoArchivo), log.IdEstado),
                FechaCreacion = log.FechaCreacion,
                Mensaje = log.Mensaje
            }).ToList();
            return logOut;
        }
    }
}

