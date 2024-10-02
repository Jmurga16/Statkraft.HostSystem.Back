using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stakraft.HostSystem.Service.Service;
using Stakraft.HostSystem.Service.ServiceDto.Parametros;

namespace Statkraft.HostSystem.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParametrosController : ControllerBase
    {
        private readonly IParametrosService _parametrosService;

        public ParametrosController(IParametrosService parametrosService)
        {
            _parametrosService = parametrosService;
        }

        [HttpPost("{nomParam}")]
        public ActionResult<ParametrosDto> ObtenerParametro(string nomParam)
        {
            var param = _parametrosService.ObtenerParametros(nomParam);
            return param;
        }

        [HttpPost]
        public ActionResult<ParametrosDto> ActualizaParametro(ParametrosDto parametroDto)
        {
            var parametro = _parametrosService.ActualizarParametros(parametroDto);
            return parametro;
        }
    }
}
