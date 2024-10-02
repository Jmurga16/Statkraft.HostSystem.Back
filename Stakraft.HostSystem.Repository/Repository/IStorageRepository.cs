using Stakraft.HostSystem.Repository.Entity;
using System.Collections.Generic;

namespace Stakraft.HostSystem.Repository.Repository
{
    public interface IStorageRepository
    {
        TbBlobStorage ObtenerTbBlobStorage(int tipoStorage, int estado);
        TbBlobStorage ObtenerTbBlobStorage(int idStorage);
        List<TbBlobStorage> ListarTbBlobStorage(int estado);
        void GuardarTbBlobStorage(TbBlobStorage tbBlobStorage);
        void InactivarStorage(TbBlobStorage tbBlobStorage);
    }
}
