using Microsoft.EntityFrameworkCore;
using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Support.SoporteEnum;

namespace Stakraft.HostSystem.Repository.Repository.Impl
{
    public class ArchivoRepository : IArchivoRepository
    {
        private readonly HostToHostDbContext _bdContext;
        public ArchivoRepository(HostToHostDbContext bdContext)
        {
            _bdContext = bdContext;
        }
        public void GuardarArchivo(Archivo archivo)
        {
            _bdContext.Archivo.Add(archivo);
            _bdContext.SaveChanges();
        }

        public void GuardarLogArchivos(LogArchivos logArchivos)
        {
            _bdContext.LogArchivos.Add(logArchivos);
            _bdContext.SaveChanges();
        }

        public Archivo ObtenerArchivo(int idArchivo)
        {
            return _bdContext.Archivo
                .Include(arc => arc.IdEstadoNavigation)
                .Include(arc => arc.BloStoIdOriginalNavigation)
                .Include(arc => arc.BloStoIdProcesadoNavigation)
                .Include(arc => arc.IdTipoPlanillaNavigation)
                .Where(arc => arc.Id.Equals(idArchivo)).FirstOrDefault();
        }

        public void UpdateArchivo(Archivo archivo)
        {
            _bdContext.Archivo.Update(archivo);
            _bdContext.SaveChanges();
        }

        public List<Archivo> ListarArchivoProcesados(string texto, int? idEstado, DateTime? fechaInicio, DateTime? fechaFin, string banco)
        {
            var query = _bdContext.Archivo
                .Include(arc => arc.IdEstadoNavigation)
                .Include(arc => arc.IdTipoPlanillaNavigation)
                .Where(arc => arc.Banco.Equals(banco));
            if (texto != null)
            {
                query = query.Where(arc => arc.NombreArchivo.Contains(texto));
            }
            if (idEstado != null)
            {
                query = query.Where(arc => arc.IdEstado.Equals(idEstado));
            }
            if (fechaInicio != null && fechaFin != null)
            {
                query = query.Where(arc => arc.FechaCreacion.Value.Date.CompareTo(fechaInicio.Value.Date) >= 0
                                        && arc.FechaCreacion.Value.Date.CompareTo(fechaFin.Value.Date) <= 1);
            }
            return query.OrderByDescending(arc => arc.FechaCreacion).ToList();
        }

        public List<Archivo> ListarArchivosPorEstado(string banco, params int[] estado)
        {
            return _bdContext.Archivo
                .Include(arc => arc.IdEstadoNavigation)
                .Include(arc => arc.BloStoIdOriginalNavigation)
                .Include(arc => arc.IdTipoPlanillaNavigation)
                .Where(arc => estado.Contains(arc.IdEstado.Value) && arc.Banco.Equals(banco)).OrderByDescending(arc => arc.FechaCreacion).ToList();
        }

        public List<SeleccionOpcion> ListarTipoPlantilla(string banco)
        {
            return _bdContext.TipoPlanilla
                .Where(tip => tip.Activo.Value && tip.Banco.Equals(banco))
                .Select(tip => new SeleccionOpcion { Id = tip.Id, Descripcion = tip.Nombre })
                .ToList();
        }


        public void DisabledAllLog(int idArchivo)
        {
            var logs = _bdContext.LogArchivos.Where(log => log.IdArchivo == idArchivo).ToList();
            logs.ForEach(log => log.IdEstado = (int)Enums.EstadoRegistro.Inactivo);
            _bdContext.SaveChanges();
        }

        public List<LogArchivos> ListarLog(string texto, int? idEstado, DateTime? fechaInicio, DateTime? fechaFin, string banco)
        {
            var query = _bdContext.LogArchivos
                .Include(log => log.IdArchivoNavigation)
                .Where(arc => arc.IdArchivoNavigation.Banco.Equals(banco));
            if (texto != null)
            {
                query = query.Where(arc => arc.IdArchivoNavigation.NombreArchivo.Contains(texto));
            }
            if (idEstado != null)
            {
                query = query.Where(arc => arc.IdEstado.Equals(idEstado));
            }
            if (fechaInicio != null && fechaFin != null)
            {
                query = query.Where(arc => arc.FechaCreacion.Value.Date.CompareTo(fechaInicio.Value.Date) >= 0
                                        && arc.FechaCreacion.Value.Date.CompareTo(fechaFin.Value.Date) <= 1);
            }
            query.OrderBy(arc => arc.IdArchivoNavigation.NombreArchivo).OrderBy(arc => arc.FechaCreacion);
            return query.ToList();
        }

    }
}
