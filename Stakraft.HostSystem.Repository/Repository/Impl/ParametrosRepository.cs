using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.RepositoryDto.Parametros;
using System.Linq;

namespace Stakraft.HostSystem.Repository.Repository.Impl
{
    public class ParametrosRepository : IParametrosRepository
    {
        private readonly HostToHostDbContext _bdContext;

        public ParametrosRepository(HostToHostDbContext bdContext)
        {
            _bdContext = bdContext;
        }

        public ParametrosRepositoryDto ObtenerParametro(string nombreParametro)
        {
            var query = _bdContext.Parametros.Select(param => new ParametrosRepositoryDto
            {
                IdParametro = param.Id,
                NombreParametro = param.NombreParametro,
                ValorParametro = param.ValorParametro
            }).Where(param => param.NombreParametro.Equals(nombreParametro)).FirstOrDefault();

            return query;
        }

        public void ActualizarParametro(Parametros paramEntinty)
        {
            _bdContext.Parametros.Update(paramEntinty);
            _bdContext.SaveChanges();
        }

        public Parametros ObtenerEntity(int idParam)
        {
            var query = _bdContext.Parametros.Where(param => param.Id.Equals(idParam)).FirstOrDefault();
            return query;
        }

        public void GuardarParametro(Parametros paramEntinty)
        {
            _bdContext.Parametros.Add(paramEntinty);
            _bdContext.SaveChanges();
        }
    }
}
