using Microsoft.AspNetCore.Http;
using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Service.ServiceDto.Resource;
using System.Threading.Tasks;

namespace Stakraft.HostSystem.Service.Service
{
    public interface IAuxBlobStorageService
    {
        Task<ResourceStorage> ObtenerArchivo(string nombreArchivo, TbBlobStorage tbBlobStorage);
        Task<ResourceStorage> ObtenerArchivoBase64(string nombreArchivo, TbBlobStorage tbBlobStorage);
        Task GuardarArchivo(string adjuntoBase64, string nombre, TbBlobStorage tbBlobStorage);
        Task GuardarArchivo(IFormFile file, string nombre, TbBlobStorage tbBlobStorage);
        Task GuardarArchivo(byte[] file, string nombre, TbBlobStorage tbBlobStorage);
        Task VerificarBlobStorageConeccion(byte[] file, string nombre, TbBlobStorage tbBlobStorage);
        void EliminarArchivo(string nombre, TbBlobStorage tbBlobStorage);
    }
}
