using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.RepositoryDto.Perfill;

namespace Stakraft.HostSystem.Repository.Repository.Impl
{
    public class PerfilRepository : IPerfilRepository
    {
        private readonly HostToHostDbContext _bdContext;

        public PerfilRepository(HostToHostDbContext bdContext)
        {
            _bdContext = bdContext;
        }

        public List<PerfilRepositoryDto> ListarPerfiles(bool activos)
        {
            var query = _bdContext.Perfiles.Select(perfil => new PerfilRepositoryDto
            {
                IdPerfil = perfil.Id,
                NombrePerfil = perfil.NombrePerfil,
                Activo = perfil.Activo
            });

            if (activos)
            {
                query = query.Where(perfil => perfil.Activo);
            }
            return query.ToList();
        }

        public void CrearPerfil(Perfiles perfil)
        {
            _bdContext.Perfiles.Add(perfil);
            _bdContext.SaveChanges();
        }

        public void EditarPerfil(Perfiles perfil)
        {
            _bdContext.Perfiles.Update(perfil);
            _bdContext.SaveChanges();
        }

        public Perfiles ObtenerPerfil(int perfilId)
        {
            var perfil = _bdContext.Perfiles.Where(perfil => perfil.Id.Equals(perfilId)).FirstOrDefault();

            return perfil;
        }
    }
}
