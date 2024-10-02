using Stakraft.HostSystem.Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stakraft.HostSystem.Repository.Repository.Impl
{
    public class TipoPlanillaRepository : ITipoPlanillaRepository
    {
        private readonly HostToHostDbContext _bdContext;
        public TipoPlanillaRepository(HostToHostDbContext bdContext)
        {
            _bdContext = bdContext;
        }
        public void Actualizar(TipoPlanilla tipoPlanilla)
        {
            _bdContext.TipoPlanilla.Update(tipoPlanilla);
            _bdContext.SaveChanges();
        }

        public bool Existe(string nombre, int? idTipoPlanilla)
        {
            var query = _bdContext.TipoPlanilla.Where(tip => tip.Nombre.Trim().ToLower().Equals(nombre.Trim().ToLower()));
            if (idTipoPlanilla != null)
            {
                query = query.Where(tip => tip.Id != idTipoPlanilla);
            }
            return query.FirstOrDefault() != null;
        }

        public void Guardar(TipoPlanilla tipoPlanilla)
        {
            _bdContext.TipoPlanilla.Add(tipoPlanilla);
            _bdContext.SaveChanges();
        }

        public bool Inactivar(int idTipoPlanilla, string usuario)
        {
            var tipoPlanilla = _bdContext.TipoPlanilla.Where(tip => tip.Id == idTipoPlanilla).FirstOrDefault();
            tipoPlanilla.Activo = !tipoPlanilla.Activo;
            tipoPlanilla.FechaModificacion = DateTime.Now;
            tipoPlanilla.UsuarioModificacion = usuario;
            _bdContext.TipoPlanilla.Update(tipoPlanilla);
            _bdContext.SaveChanges();
            return tipoPlanilla.Activo.Value;
        }

        public List<TipoPlanilla> Listar(string banco)
        {
            return _bdContext.TipoPlanilla.Where(tip => tip.Banco.Equals(banco)).ToList();
        }
    }
}
