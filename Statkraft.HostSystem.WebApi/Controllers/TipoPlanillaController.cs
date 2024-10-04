using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stakraft.HostSystem.Service.Service;
using Stakraft.HostSystem.Service.ServiceDto.TipoPlanilla;
using Stakraft.HostSystem.Support.SoporteDto;

namespace Statkraft.HostSystem.WebApi.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TipoPlanillaController : Controller
    {
        private readonly ITipoPlanillaService _tipoPlanillaService;
        public TipoPlanillaController(ITipoPlanillaService tipoPlanillaService)
        {
            _tipoPlanillaService = tipoPlanillaService;
        }

        [HttpPost("listar/{banco}")]
        public Respuesta<List<TipoPlanillaDto>> Listar(string banco)
        {
            var respuesta = new Respuesta<List<TipoPlanillaDto>>();
            var dato = _tipoPlanillaService.Listar(banco);
            return respuesta.RespuestaOperacionCompletado(dato);
        }

        [HttpPost]
        public Respuesta<TipoPlanillaDto> Guardar(TipoPlanillaDto tipoPlanilla)
        {
            var respuesta = new Respuesta<TipoPlanillaDto>();
            var dato = _tipoPlanillaService.Guardar(tipoPlanilla);
            return respuesta.RespuestaOperacionCompletado(dato);
        }

        [HttpPut()]
        public Respuesta<TipoPlanillaDto> Actualizar(TipoPlanillaDto tipoPlanilla)
        {
            var respuesta = new Respuesta<TipoPlanillaDto>();
            var dato = _tipoPlanillaService.Actualizar(tipoPlanilla);
            return respuesta.RespuestaOperacionCompletado(dato);
        }

        [HttpPut("{idTipoPlanilla}/{usuario}")]
        public Respuesta<bool> Estado(int idTipoPlanilla, string usuario)
        {
            var respuesta = new Respuesta<bool>();
            var dato = _tipoPlanillaService.Estado(idTipoPlanilla, usuario);
            return respuesta.RespuestaOperacionCompletado(dato);
        }
    }
}
