using Stakraft.HostSystem.Repository.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Stakraft.HostSystem.Repository.Repository.Impl
{
    public class StorageRepository : IStorageRepository
    {
        private readonly HostToHostDbContext _bdContext;
        public StorageRepository(HostToHostDbContext bdContext)
        {
            _bdContext = bdContext;
        }

        public void GuardarTbBlobStorage(TbBlobStorage tbBlobStorage)
        {
            _bdContext.TbBlobStorage.Add(tbBlobStorage);
            _bdContext.SaveChanges();
        }

        public void InactivarStorage(TbBlobStorage tbBlobStorage)
        {
            _bdContext.TbBlobStorage.Update(tbBlobStorage);
            _bdContext.SaveChanges();
        }

        public List<TbBlobStorage> ListarTbBlobStorage(int estado)
        {
            var query = _bdContext.TbBlobStorage.Where(sto => sto.Estado.Equals(estado));
            return query.ToList();
        }

        TbBlobStorage IStorageRepository.ObtenerTbBlobStorage(int tipoStorage, int estado)
        {
            var query = _bdContext.TbBlobStorage.Where(sto => sto.Tipo.Equals(tipoStorage) && sto.Estado.Equals(estado));
            return query.FirstOrDefault();
        }
        public TbBlobStorage ObtenerTbBlobStorage(int idStorage)
        {
            var query = _bdContext.TbBlobStorage.Where(sto => sto.Id.Equals(idStorage));
            return query.FirstOrDefault();
        }
    }
}
