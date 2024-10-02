using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.RepositoryDto.ReemplazarCaracter;
using System.Collections.Generic;
using System.Linq;

namespace Stakraft.HostSystem.Repository.Repository.Impl
{
    public class ReemplazaCaracterRepository : IReemplazaCaracterRepository
    {
        private readonly HostToHostDbContext _bdContext;

        public ReemplazaCaracterRepository(HostToHostDbContext bdContext)
        {
            _bdContext = bdContext;
        }

        public List<ReemplazarCaracterRepositoryDto> ListarCaracteres(bool activos)
        {
            var query = _bdContext.ReemplazoCaracter.Select(rCaracter => new ReemplazarCaracterRepositoryDto
            {
                IdRCaracter = rCaracter.Id,
                ValorOriginal = rCaracter.ValorOriginal,
                ValorReemplazo = rCaracter.ValorReemplazo,
                Activo = rCaracter.Activo
            });

            if (activos)
            {
                query = query.Where(rCaracter => rCaracter.Activo);
            }
            return query.ToList();
        }

        public void CrearReemplazoCaracter(ReemplazoCaracter rCaracter)
        {
            _bdContext.ReemplazoCaracter.Add(rCaracter);
            _bdContext.SaveChanges();
        }


        public void EditarReemplazoCaracter(ReemplazoCaracter rCaracter)
        {
            _bdContext.ReemplazoCaracter.Update(rCaracter);
            _bdContext.SaveChanges();
        }

        public ReemplazoCaracter ObtenerReemplazoCaracter(int rCaracterId)
        {
            var rCaracter = _bdContext.ReemplazoCaracter.Where(rCaracter => rCaracter.Id.Equals(rCaracterId)).FirstOrDefault();
            return rCaracter;
        }
    }
}
