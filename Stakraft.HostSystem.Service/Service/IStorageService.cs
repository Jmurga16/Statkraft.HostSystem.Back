using Stakraft.HostSystem.Service.ServiceDto.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stakraft.HostSystem.Service.Service
{
    public interface IStorageService
    {
        List<StorageOut> ListarStorage();
        Task GuardarStorage(StorageIn storageIn, string usuario);
        void InactivarStorage(int idStorage, string usuario);
    }
}
