using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Support.SoporteEnum;

namespace Stakraft.HostSystem.Repository.Repository.Impl
{
    public class GeneralRepository : IGeneralRepository
    {
        private readonly HostToHostDbContext _bdContext;
        public GeneralRepository(HostToHostDbContext bdContext)
        {
            _bdContext = bdContext;
        }
        public TbBlobStorage obtenerTbBlobStorage(int tipoStorage)
        {
            return _bdContext.TbBlobStorage
                    .Where(blob => blob.Estado.Equals((int)Enums.EstadoRegistro.Activo) && blob.Tipo.Equals(tipoStorage))
                    .FirstOrDefault();
        }
    }
}
