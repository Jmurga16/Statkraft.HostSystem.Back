using Microsoft.AspNetCore.Mvc;
using Stakraft.HostSystem.Support.soporte;
using Stakraft.HostSystem.Support.SoporteDto;

namespace Statkraft.HostSystem.WebApi.Controllers
{
    [Route("{*url}", Order = 9999)]
    [ApiController]
    public class NoEncontradoController : ControllerBase
    {
        [Obsolete("Metodo intercepta peticiones  a url que no existen")]
        [HttpPost]
        public Respuesta<Object> NoEncontrado()
        {
            throw new StatkraftException("Recurso no encontrado");
        }
    }
}
