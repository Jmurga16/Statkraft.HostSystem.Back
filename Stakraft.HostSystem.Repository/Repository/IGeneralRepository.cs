using Stakraft.HostSystem.Repository.Entity;

namespace Stakraft.HostSystem.Repository.Repository
{
    public interface IGeneralRepository
    {
        TbBlobStorage obtenerTbBlobStorage(int tipoStorage);
    }
}
