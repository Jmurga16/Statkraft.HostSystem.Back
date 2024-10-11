using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stakraft.HostSystem.Service.Service;
using Stakraft.HostSystem.Service.ServiceDto.ReemplazarCaracter;

namespace Statkraft.HostSystem.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReemplazarCaracterController : ControllerBase
    {
        private readonly IReemplazarCaracterService _rCaracterService;

        public ReemplazarCaracterController(IReemplazarCaracterService rCaracterService)
        {
            _rCaracterService = rCaracterService;
        }

        [HttpPost("listar")]
        public ActionResult<List<ReemplazarCaracterDto>> ListarRCaracter()
        {
            var rCaracter = _rCaracterService.ListarReemplazoCaracter(false);
            return rCaracter;
        }

        [HttpPost]
        public ActionResult<ReemplazarCaracterDto> CrearReemplazoCaracter(ReemplazarCaracterDto rCaracterDto)
        {
            var rCaracter = _rCaracterService.CrearReemplazoCaracter(rCaracterDto);
            return rCaracter;
        }

        [HttpPut("{idrCaracter}")]
        public ActionResult<ReemplazarCaracterDto> EditarReemplazoCaracter(int idrCaracter, [FromBody] ReemplazarCaracterDto rCaracterDto)
        {
            var rCaracter = _rCaracterService.EditarReemplazoCaracter(idrCaracter, rCaracterDto);
            return rCaracter;
        }

        [HttpPut("estado/{idrCaracter}")]
        public ActionResult<bool> EstadorCaracter(int idrCaracter)
        {
            var rCaracter = _rCaracterService.CambiarEstado(idrCaracter);
            return rCaracter;
        }

        [HttpPost("activos")]
        public ActionResult<List<ReemplazarCaracterDto>> ListarrCaracterActivos()
        {
            var rCaracter = _rCaracterService.ListarReemplazoCaracter(true);
            return rCaracter;
        }
    }
}
