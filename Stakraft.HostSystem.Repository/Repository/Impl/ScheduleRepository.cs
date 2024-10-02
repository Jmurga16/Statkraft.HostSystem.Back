using Microsoft.EntityFrameworkCore;
using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.RepositoryDto.Parametros;
using Stakraft.HostSystem.Repository.RepositoryDto.ReemplazarCaracter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stakraft.HostSystem.Repository.Repository.Impl
{
    public class ScheduleRepository : IScheduleRepository
    {
        public ScheduleRepository()
        {
            Console.WriteLine("Me crearon IScheduleRepository " + DateTime.Now.ToString("HH:mm"));
        }



        public LogArchivos SaveLogArchivo(HostToHostDbContext context, LogArchivos logArchivos)
        {
            context.LogArchivos.Add(logArchivos);
            context.SaveChanges();
            return logArchivos;
        }

        public ParametrosRepositoryDto ObtenerParametro(HostToHostDbContext context, string nombreParametro)
        {
            var query = context.Parametros.Select(param => new ParametrosRepositoryDto
            {
                IdParametro = param.Id,
                NombreParametro = param.NombreParametro,
                ValorParametro = param.ValorParametro
            }).Where(param => param.NombreParametro.Equals(nombreParametro)).FirstOrDefault();

            return query;
        }

        public List<Archivo> ListarArchivosPorEstado(HostToHostDbContext context, string banco, DateTime? fechaInicio, params int[] estado)
        {
            var query = context.Archivo
                 .Include(arc => arc.IdEstadoNavigation)
                 .Include(arc => arc.BloStoIdOriginalNavigation)
                 .Include(arc => arc.BloStoIdProcesadoNavigation)
                 .Include(arc => arc.IdTipoPlanillaNavigation)
                 .Where(arc => estado.Contains(arc.IdEstado.Value) && arc.Banco.Equals(banco));
            if (fechaInicio != null)
            {
                query = query.Where(arc => arc.FechaCreacion.Value.Date.CompareTo(fechaInicio.Value.Date) >= 0 ||
                arc.FechaModificacion.Value.Date.CompareTo(fechaInicio.Value.Date) >= 0);
            }
            var list = query.ToList();
            return list;
        }

        public TbBlobStorage ObtenerTbBlobStorage(HostToHostDbContext context, int tipoStorage, int estado)
        {
            var query = context.TbBlobStorage.Where(sto => sto.Tipo.Equals(tipoStorage) && sto.Estado.Equals(estado));
            return query.FirstOrDefault();
        }

        public List<ReemplazarCaracterRepositoryDto> ListarCaracteres(HostToHostDbContext context)
        {
            var query = context.ReemplazoCaracter.Select(rCaracter => new ReemplazarCaracterRepositoryDto
            {
                IdRCaracter = rCaracter.Id,
                ValorOriginal = rCaracter.ValorOriginal,
                ValorReemplazo = rCaracter.ValorReemplazo,
                Activo = rCaracter.Activo
            });

            var list = query.Where(rCaracter => rCaracter.Activo).ToList();
            return list;
        }

        public Archivo UpdateArchivo(HostToHostDbContext context, Archivo archivo)
        {
            context.Archivo.Update(archivo);
            context.SaveChanges();
            return archivo;
        }

        public void ActualizarCorrelativo(HostToHostDbContext context, string parametros)
        {
            var param = context.Parametros.Where(param => param.NombreParametro.Equals("CORRELATIVO_PLANILLA")).FirstOrDefault();
            param.ValorParametro = parametros;
            context.Parametros.Update(param);
            context.SaveChanges();
        }
    }
}
