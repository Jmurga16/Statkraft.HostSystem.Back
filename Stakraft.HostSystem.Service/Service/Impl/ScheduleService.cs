using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.Repository;
using Stakraft.HostSystem.Repository.RepositoryDto.Parametros;
using Stakraft.HostSystem.Repository.RepositoryDto.ReemplazarCaracter;
using Stakraft.HostSystem.Service.ServiceDto.Resource;
using Stakraft.HostSystem.Service.ServiceDto.SFTP;
using Stakraft.HostSystem.Support.SoporteEnum;
using Stakraft.HostSystem.Support.SoporteUtil;

namespace Stakraft.HostSystem.Service.Service.Impl
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _sheduleRepository;
        private readonly IAuxBlobStorageService _auxBlobStorage;
        private readonly IAuxFilesService _auxFile;
        private readonly IAuxFtpService _auxFTP;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger logger;

        public ScheduleService(IScheduleRepository sheduleRepository, IAuxBlobStorageService auxBlobStorage,
            IAuxFilesService auxFile, IAuxFtpService auxFTP, IServiceScopeFactory serviceScopeFactory, ILogger logger)
        {
            _auxBlobStorage = auxBlobStorage;
            _auxFile = auxFile;
            _auxFTP = auxFTP;
            _sheduleRepository = sheduleRepository;
            _serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        [AutomaticRetry(Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [DisableConcurrentExecution(timeoutInSeconds: 60)]
        public Task CrearArchivoBanco(string bei, string pais)
        {
            return Task.Run(async () =>
            {
                await ProcesarBcp();
                await ProcesarBbva(bei, pais);
            });
        }

        private async Task ProcesarBcp()
        {
            try
            {
                logger.Information("CrearArchivoBanco inicio ..." + TiempoEjecucion.fechaEjecucionProcesoBcp);
                var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<HostToHostDbContext>();
                ParametrosRepositoryDto paramBcpDto = _sheduleRepository.ObtenerParametro(dbContext, "SFTP_BANCO_BCP_IN");
                bool continuar;
                SftpDto SFTPParams;
                ConConfiguracion(paramBcpDto, out continuar, out SFTPParams);
                if (!continuar)
                {
                    return;
                }
                bool resetSerie = ValidarFechaBcp(ref continuar, SFTPParams);
                if (!continuar)
                {
                    return;
                }
                SetTiempoEjecucionBcp(dbContext, resetSerie);
                logger.Warning("CrearArchivoBanco fecha hora ejecucion ..." + DateTime.Now.ToString("dd/mm/yyyy HH:mm"));
                List<Archivo> listaArchivosReg = _sheduleRepository.ListarArchivosPorEstado(dbContext, "BCP", getFechaAnterior(), (int)Enums.EstadoArchivo.Registrado, (int)Enums.EstadoArchivo.Reingresado);
                TbBlobStorage bsProcesado = _sheduleRepository.ObtenerTbBlobStorage(dbContext, (int)Enums.TipoBlobStorage.ProcesadoBcp, (int)Enums.EstadoRegistro.Activo);
                logger.Information("CrearArchivoBanco buscando caracteres...");
                List<ReemplazarCaracterRepositoryDto> listaReemplazoCaracters = _sheduleRepository.ListarCaracteres(dbContext);
                foreach (var archivo in listaArchivosReg)
                {
                    Enums.EstadoArchivo estado = archivo.IdEstado.Equals((int)Enums.EstadoArchivo.Registrado) ? Enums.EstadoArchivo.Procesado : Enums.EstadoArchivo.Reprocesado;
                    try
                    {
                        TbBlobStorage bsOriginalArc = archivo.BloStoIdOriginalNavigation;
                        logger.Information("CrearArchivoBanco cargando storage ... " + bsOriginalArc.Container);
                        ResourceStorage resource = await _auxBlobStorage.ObtenerArchivo(archivo.NombreArchivo, bsOriginalArc);

                        string contenido = _auxFile.ObtenerContenidoArchivo(resource.FileBytes);

                        logger.Information("CrearArchivoBanco reemplazo de caracteres... " + resource.Name);

                        listaReemplazoCaracters.ForEach(rc =>
                        {
                            contenido = contenido.Replace(rc.ValorOriginal, rc.ValorReemplazo);
                        });

                        byte[] fileEdit = _auxFile.ObtenerFileEscribir(contenido);

                        logger.Information("CrearArchivoBanco guardando... " + resource.Name);

                        var nombreProcesado = ObtenerNombreArchivoBcp(archivo.IdTipoPlanillaNavigation.Prefijo);

                        await _auxBlobStorage.GuardarArchivo(fileEdit, nombreProcesado, bsProcesado);

                        logger.Information("CrearArchivoBanco actualizando Log ... " + nombreProcesado);

                        archivo.BloStoIdProcesado = bsProcesado.Id;
                        archivo.IdEstado = (int)estado;
                        archivo.NombreArchivoProcesado = nombreProcesado;
                        archivo.FechaModificacion = DateTime.Now;

                        LogArchivos log = new LogArchivos
                        {
                            IdArchivo = archivo.Id,
                            IdEstado = (int)Enums.EstadoArchivo.Procesado,
                            Activo = true,
                            FechaCreacion = DateTime.Now,
                            Mensaje = LogMensaje.getMensajeLog(Enums.EstadoArchivo.Procesado)
                        };
                        _sheduleRepository.UpdateArchivo(dbContext, archivo);
                        _sheduleRepository.SaveLogArchivo(dbContext, log);
                        var correlativo = JsonConvert.SerializeObject(TiempoEjecucion.secuenciaPlanillas);
                        _sheduleRepository.ActualizarCorrelativo(dbContext, correlativo);
                    }
                    catch (Exception e)
                    {
                        archivo.IdEstado = (int)Enums.EstadoArchivo.Erroneo;
                        LogArchivos log = new LogArchivos
                        {
                            IdArchivo = archivo.Id,
                            IdEstado = (int)Enums.EstadoArchivo.Erroneo,
                            Activo = true,
                            FechaCreacion = DateTime.Now,
                            Mensaje = LogMensaje.getMensajeError(estado)
                        };
                        var jsonArc = JsonConvert.SerializeObject(archivo, Formatting.None,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        var jsonLog = JsonConvert.SerializeObject(log, Formatting.None,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        logger.Error("CrearArchivoBanco error arc ... " + jsonArc);
                        logger.Error("CrearArchivoBanco  error log ... " + jsonLog);
                        _sheduleRepository.UpdateArchivo(dbContext, archivo);
                        _sheduleRepository.SaveLogArchivo(dbContext, log);
                        logger.Fatal(e.Message, "Error guardando archivo banco");
                    }
                }
                var correlativoString = JsonConvert.SerializeObject(TiempoEjecucion.secuenciaPlanillas);
                _sheduleRepository.ActualizarCorrelativo(dbContext, correlativoString);
                logger.Warning("CrearArchivoBanco fecha hora fin ejecucion ..." + DateTime.Now.ToString("dd/mm/yyyy HH:mm"));
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message, "Error procesando archivos banco");
            }
        }
        private async Task ProcesarBbva(string bei, string pais)
        {
            try
            {
                logger.Information("CrearArchivoBanco inicio ..." + TiempoEjecucion.fechaEjecucionProcesoBbva);
                var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<HostToHostDbContext>();
                ParametrosRepositoryDto paramBbvaDto = _sheduleRepository.ObtenerParametro(dbContext, "SFTP_BANCO_BBVA_IN");
                bool continuar;
                SftpDto SFTPParams;
                ConConfiguracion(paramBbvaDto, out continuar, out SFTPParams);
                if (!continuar)
                {
                    return;
                }
                bool resetSerie = ValidarFechaBbva(ref continuar, SFTPParams);
                if (!continuar)
                {
                    return;
                }
                SetTiempoEjecucionBbva(dbContext, resetSerie);
                logger.Warning("CrearArchivoBanco fecha hora ejecucion ..." + DateTime.Now.ToString("dd/mm/yyyy HH:mm"));
                List<Archivo> listaArchivosReg = _sheduleRepository.ListarArchivosPorEstado(dbContext, "BBVA", getFechaAnterior(), (int)Enums.EstadoArchivo.Registrado, (int)Enums.EstadoArchivo.Reingresado);
                TbBlobStorage bsProcesado = _sheduleRepository.ObtenerTbBlobStorage(dbContext, (int)Enums.TipoBlobStorage.ProcesadoBbva, (int)Enums.EstadoRegistro.Activo);
                logger.Information("CrearArchivoBanco buscando caracteres...");
                List<ReemplazarCaracterRepositoryDto> listaReemplazoCaracters = _sheduleRepository.ListarCaracteres(dbContext);
                foreach (var archivo in listaArchivosReg)
                {
                    Enums.EstadoArchivo estado = archivo.IdEstado.Equals((int)Enums.EstadoArchivo.Registrado) ? Enums.EstadoArchivo.Procesado : Enums.EstadoArchivo.Reprocesado;
                    try
                    {
                        TbBlobStorage bsOriginalArc = archivo.BloStoIdOriginalNavigation;
                        logger.Information("CrearArchivoBanco cargando storage ... " + bsOriginalArc.Container);
                        ResourceStorage resource = await _auxBlobStorage.ObtenerArchivo(archivo.NombreArchivo, bsOriginalArc);

                        string contenido = _auxFile.ObtenerContenidoArchivo(resource.FileBytes);

                        logger.Information("CrearArchivoBanco reemplazo de caracteres... " + resource.Name);

                        listaReemplazoCaracters.ForEach(rc =>
                        {
                            contenido = contenido.Replace(rc.ValorOriginal, rc.ValorReemplazo);
                        });

                        byte[] fileEdit = _auxFile.ObtenerFileEscribir(contenido);

                        logger.Information("CrearArchivoBanco guardando... " + resource.Name);

                        var nombreProcesado = ObtenerNombreArchivoBbva(bei, pais, archivo.IdTipoPlanillaNavigation.Prefijo);

                        await _auxBlobStorage.GuardarArchivo(fileEdit, nombreProcesado, bsProcesado);

                        logger.Information("CrearArchivoBanco actualizando Log ... " + nombreProcesado);

                        archivo.BloStoIdProcesado = bsProcesado.Id;
                        archivo.IdEstado = (int)estado;
                        archivo.NombreArchivoProcesado = nombreProcesado;
                        archivo.FechaModificacion = DateTime.Now;

                        LogArchivos log = new LogArchivos
                        {
                            IdArchivo = archivo.Id,
                            IdEstado = (int)Enums.EstadoArchivo.Procesado,
                            Activo = true,
                            FechaCreacion = DateTime.Now,
                            Mensaje = LogMensaje.getMensajeLog(Enums.EstadoArchivo.Procesado)
                        };
                        _sheduleRepository.UpdateArchivo(dbContext, archivo);
                        _sheduleRepository.SaveLogArchivo(dbContext, log);
                        var correlativo = JsonConvert.SerializeObject(TiempoEjecucion.secuenciaPlanillas);
                        _sheduleRepository.ActualizarCorrelativo(dbContext, correlativo);
                    }
                    catch (Exception e)
                    {
                        archivo.IdEstado = (int)Enums.EstadoArchivo.Erroneo;
                        LogArchivos log = new LogArchivos
                        {
                            IdArchivo = archivo.Id,
                            IdEstado = (int)Enums.EstadoArchivo.Erroneo,
                            Activo = true,
                            FechaCreacion = DateTime.Now,
                            Mensaje = LogMensaje.getMensajeError(estado)
                        };
                        var jsonArc = JsonConvert.SerializeObject(archivo, Formatting.None,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        var jsonLog = JsonConvert.SerializeObject(log, Formatting.None,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        logger.Error("CrearArchivoBanco error arc ... " + jsonArc);
                        logger.Error("CrearArchivoBanco  error log ... " + jsonLog);
                        _sheduleRepository.UpdateArchivo(dbContext, archivo);
                        _sheduleRepository.SaveLogArchivo(dbContext, log);
                        logger.Fatal(e.Message, "Error guardando archivo banco");
                    }
                }
                var correlativoString = JsonConvert.SerializeObject(TiempoEjecucion.secuenciaPlanillas);
                _sheduleRepository.ActualizarCorrelativo(dbContext, correlativoString);
                logger.Warning("CrearArchivoBanco fecha hora fin ejecucion ..." + DateTime.Now.ToString("dd/mm/yyyy HH:mm"));
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message, "Error procesando archivos banco");
            }
        }

        private void SetTiempoEjecucionBcp(HostToHostDbContext dbContext, bool resetSerie)
        {
            TiempoEjecucion.fechaEjecucionProcesoBcp = DateTime.Now;
            ParametrosRepositoryDto paramCorrelativo = _sheduleRepository.ObtenerParametro(dbContext, "CORRELATIVO_PLANILLA");
            if (paramCorrelativo != null && paramCorrelativo.ValorParametro != null)
            {
                TiempoEjecucion.secuenciaPlanillas = JsonConvert.DeserializeObject<Dictionary<string, int>>(paramCorrelativo.ValorParametro);
            }
            SetSecuencia(resetSerie);
        }
        private void SetTiempoEjecucionBbva(HostToHostDbContext dbContext, bool resetSerie)
        {
            TiempoEjecucion.fechaEjecucionProcesoBbva = DateTime.Now;
            ParametrosRepositoryDto paramCorrelativo = _sheduleRepository.ObtenerParametro(dbContext, "CORRELATIVO_PLANILLA");
            if (paramCorrelativo != null && paramCorrelativo.ValorParametro != null)
            {
                TiempoEjecucion.secuenciaPlanillas = JsonConvert.DeserializeObject<Dictionary<string, int>>(paramCorrelativo.ValorParametro);
            }
            SetSecuencia(resetSerie);
        }

        private bool ValidarFechaBcp(ref bool continuar, SftpDto SFTPParams)
        {
            var fechaActual = DateTime.Now;
            var horaActual = new TimeSpan(fechaActual.Ticks);
            var intervaloProceso = new TimeSpan(0, SFTPParams.IntervaloProceso.Value, 0);
            var horaActualIntervalo = horaActual + intervaloProceso;
            bool resetSerie = true;
            if (horaActual.CompareTo(SFTPParams.HoraInicio.Value) == -1 || SFTPParams.HoraFin.Value.CompareTo(horaActualIntervalo) > 0)
            {
                continuar = false;
            }
            if (TiempoEjecucion.fechaEjecucionProcesoBcp.Date.CompareTo(fechaActual.Date) == 0)
            {
                var horaProximaEjecucion = new TimeSpan(TiempoEjecucion.fechaEjecucionProcesoBcp.Ticks) + intervaloProceso;
                if (horaActual.CompareTo(horaProximaEjecucion) == -1)
                {
                    logger.Information("CrearArchivoBanco fuera de intervalo de ejecucion...");
                    continuar = false;
                }
                resetSerie = false;
            }

            return resetSerie;
        }
        private bool ValidarFechaBbva(ref bool continuar, SftpDto SFTPParams)
        {
            var fechaActual = DateTime.Now;
            var horaActual = new TimeSpan(fechaActual.Ticks);
            var intervaloProceso = new TimeSpan(0, SFTPParams.IntervaloProceso.Value, 0);
            var horaActualIntervalo = horaActual + intervaloProceso;
            bool resetSerie = true;
            if (horaActual.CompareTo(SFTPParams.HoraInicio.Value) == -1 || SFTPParams.HoraFin.Value.CompareTo(horaActualIntervalo) > 0)
            {
                continuar = false;
            }
            if (horaActual.CompareTo(SFTPParams.HoraInicio2.Value) == -1 || SFTPParams.HoraFin2.Value.CompareTo(horaActualIntervalo) > 0)
            {
                continuar = false;
            }
            if (TiempoEjecucion.fechaEjecucionProcesoBbva.Date.CompareTo(fechaActual.Date) == 0)
            {
                var horaProximaEjecucion = new TimeSpan(TiempoEjecucion.fechaEjecucionProcesoBbva.Ticks) + intervaloProceso;
                if (horaActual.CompareTo(horaProximaEjecucion) == -1)
                {
                    logger.Information("CrearArchivoBanco fuera de intervalo de ejecucion...");
                    continuar = false;
                }
                resetSerie = false;
            }

            return resetSerie;
        }

        private void ConConfiguracion(ParametrosRepositoryDto paramDto, out bool continuar, out SftpDto SFTPParams)
        {
            continuar = true;
            if (paramDto != null)
            {
                var valor = UtilCifrado.Desencripta(paramDto.ValorParametro);
                SFTPParams = JsonConvert.DeserializeObject<SftpDto>(valor);
                if (SFTPParams.IntervaloProceso == null)
                {
                    logger.Information("TransferirArchivosSFTP Sin configuracion de IntervaloProceso  del banco IN...");
                    continuar = false;
                }
                logger.Information("CrearArchivoBanco sin configuracion banco IN...");
            }
            else
            {
                SFTPParams = null;
                continuar = false;
            }

        }

        [AutomaticRetry(Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [DisableConcurrentExecution(timeoutInSeconds: 60)]
        public Task TransferirArchivosSFTP()
        {
            return Task.Run(async () =>
            {
                await TransferirBcp();
                await TransferirBbva();
            });
        }

        private async Task TransferirBcp()
        {
            try
            {
                logger.Information("TransferirArchivosSFTP inicio ..." + TiempoEjecucion.fechaEjecucionProcesoBcp);
                var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<HostToHostDbContext>();
                ParametrosRepositoryDto paramDto = _sheduleRepository.ObtenerParametro(dbContext, "SFTP_BANCO_BCP_IN");
                bool continuar = true;
                SftpDto SFTPParams = ConconfiguracionSftp(paramDto, ref continuar);
                if (!continuar)
                {
                    return;
                }
                var fechaActual = DateTime.Now;
                var horaActual = new TimeSpan(fechaActual.Ticks);
                var intervaloEnvio = new TimeSpan(0, SFTPParams.Intervalo.Value, 0);
                var horaActualIntervalo = horaActual + intervaloEnvio;
                continuar = ValidarFechaSftpBcp(continuar, SFTPParams, fechaActual, horaActual, intervaloEnvio, horaActualIntervalo);
                if (!continuar)
                {
                    return;
                }
                TiempoEjecucion.fechaEjecucionEnvioBancoInBcp = DateTime.Now;
                logger.Warning("TransferirArchivosSFTP fecha hora ejecucion ..." + DateTime.Now.ToString("dd/mm/yyyy HH:mm"));
                List<Archivo> listaArchivosTransferir = _sheduleRepository.ListarArchivosPorEstado(dbContext, "BCP", getFechaAnterior(), (int)Enums.EstadoArchivo.Procesado, (int)Enums.EstadoArchivo.Reprocesado);
                logger.Information("TransferirArchivosSFTP cargando configuracion sftp ... " + SFTPParams.PathServidor);
                await GuardarArchivos(dbContext, SFTPParams, listaArchivosTransferir);
                logger.Warning("TransferirArchivosSFTP fecha hora fin ejecucion ..." + DateTime.Now.ToString("dd/mm/yyyy HH:mm"));
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message, "Error procesando archivos banco");
            }
        }
        private async Task TransferirBbva()
        {
            try
            {
                logger.Information("TransferirArchivosSFTP inicio ..." + TiempoEjecucion.fechaEjecucionEnvioBancoInBbva);
                var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<HostToHostDbContext>();
                ParametrosRepositoryDto paramDto = _sheduleRepository.ObtenerParametro(dbContext, "SFTP_BANCO_BBVA_IN");
                bool continuar = true;
                SftpDto SFTPParams = ConconfiguracionSftp(paramDto, ref continuar);
                if (!continuar)
                {
                    return;
                }
                var fechaActual = DateTime.Now;
                var horaActual = new TimeSpan(fechaActual.Ticks);
                var intervaloEnvio = new TimeSpan(0, SFTPParams.Intervalo.Value, 0);
                var horaActualIntervalo = horaActual + intervaloEnvio;
                continuar = ValidarFechaSftpBbva(continuar, SFTPParams, fechaActual, horaActual, intervaloEnvio, horaActualIntervalo);
                if (!continuar)
                {
                    return;
                }
                TiempoEjecucion.fechaEjecucionEnvioBancoInBbva = DateTime.Now;
                logger.Warning("TransferirArchivosSFTP fecha hora ejecucion ..." + DateTime.Now.ToString("dd/mm/yyyy HH:mm"));
                List<Archivo> listaArchivosTransferir = _sheduleRepository.ListarArchivosPorEstado(dbContext, "BBVA", getFechaAnterior(), (int)Enums.EstadoArchivo.Procesado, (int)Enums.EstadoArchivo.Reprocesado);
                logger.Information("TransferirArchivosSFTP cargando configuracion sftp ... " + SFTPParams.PathServidor);
                await GuardarArchivos(dbContext, SFTPParams, listaArchivosTransferir);
                logger.Warning("TransferirArchivosSFTP fecha hora fin ejecucion ..." + DateTime.Now.ToString("dd/mm/yyyy HH:mm"));
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message, "Error procesando archivos banco");
            }
        }

        private async Task GuardarArchivos(HostToHostDbContext dbContext, SftpDto SFTPParams, List<Archivo> listaArchivosTransferir)
        {
            foreach (var archivo in listaArchivosTransferir)
            {
                Enums.EstadoArchivo estado = archivo.IdEstado.Equals((int)Enums.EstadoArchivo.Procesado) ? Enums.EstadoArchivo.Enviado : Enums.EstadoArchivo.Reenviado;
                try
                {
                    TbBlobStorage bsProcesadoArc = archivo.BloStoIdProcesadoNavigation;
                    ResourceStorage resource = await _auxBlobStorage.ObtenerArchivo(archivo.NombreArchivoProcesado, bsProcesadoArc);
                    bool transferido = _auxFTP.TransferFile(resource.Name, resource.FileBytes, SFTPParams);
                    logger.Information("TransferirArchivosSFTP resultado envio sftp ... " + transferido);
                    if (transferido)
                    {
                        logger.Information("TransferirArchivosSFTP actualizando log archivo... " + transferido);
                        archivo.IdEstado = (int)estado;
                        archivo.FechaModificacion = DateTime.Now;

                        LogArchivos log = new LogArchivos
                        {
                            IdArchivo = archivo.Id,
                            IdEstado = (int)estado,
                            Activo = true,
                            FechaCreacion = DateTime.Now,
                            Mensaje = LogMensaje.getMensajeLog(estado)
                        };
                        _sheduleRepository.UpdateArchivo(dbContext, archivo);
                        _sheduleRepository.SaveLogArchivo(dbContext, log);
                    }
                    logger.Information("TransferirArchivosSFTP finalizando envio archivo... " + archivo.NombreArchivo);
                }
                catch (Exception e)
                {
                    archivo.IdEstado = (int)Enums.EstadoArchivo.Erroneo;
                    LogArchivos log = new LogArchivos
                    {
                        IdArchivo = archivo.Id,
                        IdEstado = (int)Enums.EstadoArchivo.Erroneo,
                        Activo = true,
                        FechaCreacion = DateTime.Now,
                        Mensaje = LogMensaje.getMensajeError(estado)
                    };
                    _sheduleRepository.UpdateArchivo(dbContext, archivo);
                    _sheduleRepository.SaveLogArchivo(dbContext, log);
                    var jsonArc = JsonConvert.SerializeObject(archivo, Formatting.None,
                      new JsonSerializerSettings()
                      {
                          ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                      });
                    var jsonLog = JsonConvert.SerializeObject(log, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                    logger.Error("CrearArchivoBanco error arc ... " + jsonArc);
                    logger.Error("CrearArchivoBanco  error log ... " + jsonLog);
                    logger.Fatal(e.Message, "Error enviando archivos banco");
                }
            }
        }

        private static bool ValidarFechaSftpBcp(bool continuar, SftpDto SFTPParams, DateTime fechaActual, TimeSpan horaActual, TimeSpan intervaloEnvio, TimeSpan horaActualIntervalo)
        {
            if (horaActual.CompareTo(SFTPParams.HoraInicio.Value) == -1 || SFTPParams.HoraFin.Value.CompareTo(horaActualIntervalo) > 0)
            {
                continuar = false;
            }
            if (TiempoEjecucion.fechaEjecucionEnvioBancoInBcp.Date.CompareTo(fechaActual.Date) == 0)
            {
                var horaProximaEjecucion = new TimeSpan(TiempoEjecucion.fechaEjecucionEnvioBancoInBcp.Ticks) + intervaloEnvio;
                if (horaActual.CompareTo(horaProximaEjecucion) == -1)
                {
                    continuar = false;
                }
            }

            return continuar;
        }
        private static bool ValidarFechaSftpBbva(bool continuar, SftpDto SFTPParams, DateTime fechaActual, TimeSpan horaActual, TimeSpan intervaloEnvio, TimeSpan horaActualIntervalo)
        {
            if (horaActual.CompareTo(SFTPParams.HoraInicio.Value) == -1 || SFTPParams.HoraFin.Value.CompareTo(horaActualIntervalo) > 0)
            {
                continuar = false;
            }
            if (horaActual.CompareTo(SFTPParams.HoraInicio2.Value) == -1 || SFTPParams.HoraFin2.Value.CompareTo(horaActualIntervalo) > 0)
            {
                continuar = false;
            }
            if (TiempoEjecucion.fechaEjecucionEnvioBancoInBbva.Date.CompareTo(fechaActual.Date) == 0)
            {
                var horaProximaEjecucion = new TimeSpan(TiempoEjecucion.fechaEjecucionEnvioBancoInBbva.Ticks) + intervaloEnvio;
                if (horaActual.CompareTo(horaProximaEjecucion) == -1)
                {
                    continuar = false;
                }
            }

            return continuar;
        }

        private SftpDto ConconfiguracionSftp(ParametrosRepositoryDto paramDto, ref bool continuar)
        {
            if (paramDto == null)
            {
                logger.Information("TransferirArchivosSFTP Sin configuracion banco IN...");
                continuar = false;
                return null;
            }
            var valor = UtilCifrado.Desencripta(paramDto.ValorParametro);
            SftpDto SFTPParams = JsonConvert.DeserializeObject<SftpDto>(valor);
            if (SFTPParams.HoraInicio == null || SFTPParams.HoraFin == null || SFTPParams.Intervalo == null)
            {
                logger.Information("TransferirArchivosSFTP Sin configuracion de rango de atencion e intervalo de envio del banco IN...");
                continuar = false;
                return null;
            }
            return SFTPParams;
        }

        [AutomaticRetry(Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [DisableConcurrentExecution(timeoutInSeconds: 60)]
        public Task LeerRespuestaOutSFTP(String prefixOut, string prefixRecibido, string prefixInvalido, string prefixAutorizado)
        {
            return Task.Run(() =>
            {
                LeerRespuestaOutBcp(prefixOut);
                LeerRespuestaOutBbva(prefixRecibido, prefixInvalido, prefixAutorizado);
            });
        }
        private void LeerRespuestaOutBcp(string prefixOut)
        {
            try
            {
                logger.Information("LeerRespuestaOutSFTP inicio ..." + TiempoEjecucion.fechaEjecucionProcesoBcp);
                var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<HostToHostDbContext>();
                ParametrosRepositoryDto paramDto = _sheduleRepository.ObtenerParametro(dbContext, "SFTP_BANCO_BCP_OUT");
                var continuar = true;
                SftpDto SFTPParams = ValidarParamOutSftp(paramDto, ref continuar);
                if (!continuar)
                {
                    return;
                }
                continuar = SetFechaOutSftpBcp(continuar, SFTPParams);
                if (!continuar)
                {
                    return;
                }
                TiempoEjecucion.fechaEjecucionLecturaBancoOutBcp = DateTime.Now;
                logger.Warning("LeerRespuestaOutSFTP fecha hora ejecucion ..." + DateTime.Now.ToString("dd/mm/yyyy HH:mm"));
                List<Archivo> listaArchivosTransferir = _sheduleRepository.ListarArchivosPorEstado(dbContext, "BCP", getFechaAnterior(), (int)Enums.EstadoArchivo.Enviado, (int)Enums.EstadoArchivo.Reenviado);
                listaArchivosTransferir.ForEach(archivo =>
                {
                    var nombre = archivo.NombreArchivoProcesado.Replace(".txt", prefixOut + ".txt");
                    logger.Information("LeerRespuestaOutSFTP buscando OUT ..." + nombre);
                    try
                    {
                        var contenido = _auxFTP.getContentFileOut(SFTPParams, nombre);
                        if (contenido != null)
                        {
                            logger.Information("LeerRespuestaOutSFTP inicio registro error OUT ..." + nombre);
                            archivo.IdEstado = (int)Enums.EstadoArchivo.Rechazado;
                            archivo.FechaModificacion = DateTime.Now;
                            archivo.MenError = contenido;
                            LogArchivos log = new LogArchivos
                            {
                                IdArchivo = archivo.Id,
                                IdEstado = (int)archivo.IdEstado,
                                Activo = true,
                                FechaCreacion = DateTime.Now,
                                Mensaje = LogMensaje.getMensajeLog(Enums.EstadoArchivo.Rechazado)
                            };
                            _sheduleRepository.UpdateArchivo(dbContext, archivo);
                            _sheduleRepository.SaveLogArchivo(dbContext, log);
                            logger.Information("LeerRespuestaOutSFTP fin regitro error OUT ..." + nombre);
                        }
                    }
                    catch (Exception e)
                    {
                        archivo.IdEstado = (int)Enums.EstadoArchivo.Erroneo;
                        LogArchivos log = new LogArchivos
                        {
                            IdArchivo = archivo.Id,
                            IdEstado = (int)Enums.EstadoArchivo.Erroneo,
                            Activo = true,
                            FechaCreacion = DateTime.Now,
                            Mensaje = LogMensaje.getMensajeError(Enums.EstadoArchivo.Rechazado)
                        };
                        _sheduleRepository.UpdateArchivo(dbContext, archivo);
                        _sheduleRepository.SaveLogArchivo(dbContext, log);
                        var jsonArc = JsonConvert.SerializeObject(archivo, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                        var jsonLog = JsonConvert.SerializeObject(log, Formatting.None,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        logger.Error("CrearArchivoBanco error arc ... " + jsonArc);
                        logger.Error("CrearArchivoBanco  error log ... " + jsonLog);
                        logger.Warning(e.Message, "Error procesando archivos respuesta banco");
                    }
                    logger.Information("LeerRespuestaOutSFTP fin buscando OUT ..." + archivo.NombreArchivoProcesado);
                });
                logger.Warning("LeerRespuestaOutSFTP fecha hora fin ejecucion ..." + DateTime.Now.ToString("dd/mm/yyyy HH:mm"));
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message, "Error procesando archivos banco");
            }
        }
        private void LeerRespuestaOutBbva(string prefixRecibido, string prefixInvalido, string prefixAutorizado)
        {
            try
            {
                logger.Information("LeerRespuestaOutSFTP inicio ..." + TiempoEjecucion.fechaEjecucionProcesoBbva);
                var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<HostToHostDbContext>();
                ParametrosRepositoryDto paramDto = _sheduleRepository.ObtenerParametro(dbContext, "SFTP_BANCO_BBVA_OUT");
                var continuar = true;
                SftpDto SFTPParams = ValidarParamOutSftp(paramDto, ref continuar);
                if (!continuar)
                {
                    return;
                }
                continuar = SetFechaOutSftpBbva(continuar, SFTPParams);
                if (!continuar)
                {
                    return;
                }
                TiempoEjecucion.fechaEjecucionLecturaBancoOutBbva = DateTime.Now;
                logger.Warning("LeerRespuestaOutSFTP fecha hora ejecucion ..." + DateTime.Now.ToString("dd/mm/yyyy HH:mm"));
                List<Archivo> listaArchivosTransferir = _sheduleRepository
                    .ListarArchivosPorEstado(dbContext, "BBVA",
                    getFechaAnterior(),
                    (int)Enums.EstadoArchivo.Enviado,
                    (int)Enums.EstadoArchivo.Reenviado,
                    (int)Enums.EstadoArchivo.Recepcionado
                    );
                listaArchivosTransferir.ForEach(archivo =>
                {
                    var nombre = archivo.NombreArchivoProcesado;
                    logger.Information("LeerRespuestaOutSFTP buscando OUT ..." + nombre);
                    try
                    {

                        var contenidoRecibido = _auxFTP.getContentFileOutBbva(SFTPParams, nombre + prefixRecibido);
                        var contenidoInvalido = _auxFTP.getContentFileOutBbva(SFTPParams, nombre + prefixInvalido);
                        var contenidoAutorizado = _auxFTP.getContentFileOutBbva(SFTPParams, nombre + prefixAutorizado);
                        var mensajeError = "";
                        Enums.EstadoArchivo estado = 0;
                        if (contenidoRecibido != null && !archivo.IdEstado.Equals((int)Enums.EstadoArchivo.Recepcionado))
                        {
                            estado = Enums.EstadoArchivo.Recepcionado;
                            mensajeError = contenidoRecibido;
                        }
                        if (contenidoInvalido != null)
                        {
                            estado = Enums.EstadoArchivo.Rechazado;
                            mensajeError = contenidoInvalido;
                        }
                        if (contenidoAutorizado != null)
                        {
                            estado = Enums.EstadoArchivo.Autorizado;
                            mensajeError = contenidoAutorizado;
                        }
                        if (estado != 0)
                        {
                            logger.Information("LeerRespuestaOutSFTP inicio registro error OUT ..." + nombre);
                            archivo.IdEstado = (int)estado;
                            archivo.FechaModificacion = DateTime.Now;
                            archivo.MenError = mensajeError;
                            LogArchivos log = new LogArchivos
                            {
                                IdArchivo = archivo.Id,
                                IdEstado = (int)archivo.IdEstado,
                                Activo = true,
                                FechaCreacion = DateTime.Now,
                                Mensaje = LogMensaje.getMensajeLog(Enums.EstadoArchivo.Rechazado)
                            };
                            _sheduleRepository.UpdateArchivo(dbContext, archivo);
                            _sheduleRepository.SaveLogArchivo(dbContext, log);
                            logger.Information("LeerRespuestaOutSFTP fin regitro error OUT ..." + nombre);
                        }
                    }
                    catch (Exception e)
                    {
                        archivo.IdEstado = (int)Enums.EstadoArchivo.Erroneo;
                        LogArchivos log = new LogArchivos
                        {
                            IdArchivo = archivo.Id,
                            IdEstado = (int)Enums.EstadoArchivo.Erroneo,
                            Activo = true,
                            FechaCreacion = DateTime.Now,
                            Mensaje = LogMensaje.getMensajeError(Enums.EstadoArchivo.Rechazado)
                        };
                        _sheduleRepository.UpdateArchivo(dbContext, archivo);
                        _sheduleRepository.SaveLogArchivo(dbContext, log);
                        var jsonArc = JsonConvert.SerializeObject(archivo, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                        var jsonLog = JsonConvert.SerializeObject(log, Formatting.None,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                        logger.Error("CrearArchivoBanco error arc ... " + jsonArc);
                        logger.Error("CrearArchivoBanco  error log ... " + jsonLog);
                        logger.Warning(e.Message, "Error procesando archivos respuesta banco");
                    }
                    logger.Information("LeerRespuestaOutSFTP fin buscando OUT ..." + archivo.NombreArchivoProcesado);
                });
                logger.Warning("LeerRespuestaOutSFTP fecha hora fin ejecucion ..." + DateTime.Now.ToString("dd/mm/yyyy HH:mm"));
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message, "Error procesando archivos banco");
            }
        }
        private bool SetFechaOutSftpBcp(bool continuar, SftpDto SFTPParams)
        {
            var fechaActual = DateTime.Now;
            var horaActual = new TimeSpan(fechaActual.Ticks);
            var intervaloEnvio = new TimeSpan(0, SFTPParams.Intervalo.Value, 0);
            var horaActualIntervalo = horaActual + intervaloEnvio;

            if (horaActual.CompareTo(SFTPParams.HoraInicio.Value) == -1 || SFTPParams.HoraFin.Value.CompareTo(horaActualIntervalo) > 0)
            {
                logger.Information("LeerRespuestaOutSFTP Fuera de rango de atencion banco OUT...");
                continuar = false;
            }
            if (TiempoEjecucion.fechaEjecucionLecturaBancoOutBcp.Date.CompareTo(fechaActual.Date) == 0)
            {
                var horaProximaEjecucion = new TimeSpan(TiempoEjecucion.fechaEjecucionLecturaBancoOutBcp.Ticks) + intervaloEnvio;
                if (horaActual.CompareTo(horaProximaEjecucion) == -1)
                {
                    logger.Information("LeerRespuestaOutSFTP fuera de intervalo de lectura de errores ...");
                    continuar = false;
                }
            }

            return continuar;
        }
        private bool SetFechaOutSftpBbva(bool continuar, SftpDto SFTPParams)
        {
            var fechaActual = DateTime.Now;
            var horaActual = new TimeSpan(fechaActual.Ticks);
            var intervaloEnvio = new TimeSpan(0, SFTPParams.Intervalo.Value, 0);
            var horaActualIntervalo = horaActual + intervaloEnvio;

            if (horaActual.CompareTo(SFTPParams.HoraInicio.Value) == -1 || SFTPParams.HoraFin.Value.CompareTo(horaActualIntervalo) > 0)
            {
                logger.Information("LeerRespuestaOutSFTP Fuera de rango de atencion banco OUT...");
                continuar = false;
            }
            if (horaActual.CompareTo(SFTPParams.HoraInicio2.Value) == -1 || SFTPParams.HoraFin2.Value.CompareTo(horaActualIntervalo) > 0)
            {
                logger.Information("LeerRespuestaOutSFTP Fuera de rango de atencion banco OUT...");
                continuar = false;
            }
            if (TiempoEjecucion.fechaEjecucionLecturaBancoOutBbva.Date.CompareTo(fechaActual.Date) == 0)
            {
                var horaProximaEjecucion = new TimeSpan(TiempoEjecucion.fechaEjecucionLecturaBancoOutBbva.Ticks) + intervaloEnvio;
                if (horaActual.CompareTo(horaProximaEjecucion) == -1)
                {
                    logger.Information("LeerRespuestaOutSFTP fuera de intervalo de lectura de errores ...");
                    continuar = false;
                }
            }

            return continuar;
        }
        private SftpDto ValidarParamOutSftp(ParametrosRepositoryDto paramDto, ref bool continuar)
        {
            if (paramDto == null)
            {
                logger.Information("LeerRespuestaOutSFTP Sin configuracion banco OUT...");
                continuar = false;
                return null;
            }
            var valor = UtilCifrado.Desencripta(paramDto.ValorParametro);
            SftpDto SFTPParams = JsonConvert.DeserializeObject<SftpDto>(valor);
            if (SFTPParams.HoraInicio == null || SFTPParams.HoraFin == null || SFTPParams.Intervalo == null)
            {
                logger.Information("LeerRespuestaOutSFTP Sin configuracion de rango de atencion e intervalo de envio del banco OUT...");
                continuar = false;
                return null;
            }
            return SFTPParams;
        }
        private string ObtenerNombreArchivoBcp(string prefixTipoPlanilla)
        {
            var secuenciaNueva = ObtenerSecuencia(prefixTipoPlanilla);
            var serie = "000000" + secuenciaNueva;
            var indexStart = serie.Length - 6;
            var indexFinal = 6;
            var serieFinal = serie.Substring(indexStart, indexFinal);
            var nombre = prefixTipoPlanilla + DateTime.Now.ToString("yyyyMMdd") + serieFinal + "P.txt";
            return nombre;
        }
        private string ObtenerNombreArchivoBbva(string bei, string pais, string plantillaPrefix)
        {
            var secuenciaNueva = ObtenerSecuencia(plantillaPrefix);
            var serie = "000" + secuenciaNueva;
            var indexStart = serie.Length - 3;
            var indexFinal = 3;
            var serieFinal = serie.Substring(indexStart, indexFinal);
            var nombre = bei + "_" + pais + "_" + DateTime.Now.ToString("yyMMdd") + serieFinal + "." + plantillaPrefix;
            return nombre;
        }
        private void SetSecuencia(bool resetSerie)
        {
            if (resetSerie)
            {
                var newCorrelativo = new Dictionary<string, int>();
                foreach (KeyValuePair<string, int> item in TiempoEjecucion.secuenciaPlanillas)
                {
                    newCorrelativo.Add(item.Key, 0);
                }
                TiempoEjecucion.secuenciaPlanillas = newCorrelativo;
            }
            logger.Warning("resetSerie" + resetSerie);
            var correlativo = JsonConvert.SerializeObject(TiempoEjecucion.secuenciaPlanillas);
            logger.Warning("resetSerie" + correlativo);
        }
        private int ObtenerSecuencia(string prefixTipoPlanilla)
        {
            var contieneSecuencia = TiempoEjecucion.secuenciaPlanillas.ContainsKey(prefixTipoPlanilla);
            if (contieneSecuencia)
            {
                var secuencia = TiempoEjecucion.secuenciaPlanillas[prefixTipoPlanilla];
                var nuevaSecuencia = secuencia + 1;
                TiempoEjecucion.secuenciaPlanillas[prefixTipoPlanilla] = nuevaSecuencia;
                return nuevaSecuencia;
            }
            else
            {
                TiempoEjecucion.secuenciaPlanillas.Add(prefixTipoPlanilla, 1);
                return 1;
            }
        }
        private DateTime getFechaAnterior()
        {
            var fechaActual = DateTime.Now;
            var fechaAyer = fechaActual.AddDays(-1);
            return fechaAyer;
        }
    }
}
