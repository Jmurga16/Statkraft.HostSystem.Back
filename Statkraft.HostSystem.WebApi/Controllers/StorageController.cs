using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stakraft.HostSystem.Service.Service;
using Stakraft.HostSystem.Service.ServiceDto.Storage;
using Stakraft.HostSystem.Support.SoporteDto;

namespace Statkraft.HostSystem.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;
        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("listar")]
        public Respuesta<List<StorageOut>> ListarStorage()
        {
            var respuesta = new Respuesta<List<StorageOut>>();
            var dato = _storageService.ListarStorage();
            return respuesta.RespuestaCorrectaListar(dato);
        }

        [HttpPost]
        public async Task<Respuesta<Object>> GuardarStorage(StorageIn storageIn)
        {
            var respuesta = new Respuesta<Object>();
            await _storageService.GuardarStorage(storageIn, storageIn.Usuario);
            return respuesta.RespuestaCorrectaActualizar(null);
        }

        [HttpPut("{idStorage}/{usuario}")]
        public Respuesta<Object> InactivarStorage(int idStorage, string usuario)
        {
            var respuesta = new Respuesta<Object>();
            _storageService.InactivarStorage(idStorage, usuario);
            return respuesta.RespuestaCorrectaInactivar(null);
        }

    }
}
