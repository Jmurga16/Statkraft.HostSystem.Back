using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stakraft.HostSystem.Service.Service;
using Stakraft.HostSystem.Service.ServiceDto.Perfil;

namespace Statkraft.HostSystem.WebApi.Controllers
{
    [Authorize]
    [Route("mantenimiento/[controller]")]
    [ApiController]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilService _perfilService;

        public PerfilController(IPerfilService perfilService)
        {
            _perfilService = perfilService;
        }

        [HttpPost("listar")]
        public ActionResult<List<PerfilDto>> ListarPerfiles(string dato)
        {
            var perfiles = _perfilService.ListarPerfiles(false);
            return perfiles;
        }

        [HttpPost]
        public ActionResult<PerfilDto> CrearPerfil(PerfilDto perfilDto)
        {
            var perfil = _perfilService.CrearPerfil(perfilDto);
            return perfil;
        }

        [HttpPut("{idPerfil}")]
        public ActionResult<PerfilDto> EditarPerfil(int idPerfil, [FromBody] PerfilDto perfilDto)
        {
            var perfil = _perfilService.EditarPerfil(idPerfil, perfilDto);
            return perfil;
        }

        [HttpPut("estado/{idPerfil}")]
        public ActionResult<bool> EstadoPerfil(int idPerfil)
        {
            var perfil = _perfilService.CambiarEstado(idPerfil);
            return perfil;
        }

        [HttpPost("activos")]
        public ActionResult<List<PerfilDto>> ListarPerfilesActivos()
        {
            var perfiles = _perfilService.ListarPerfiles(true);
            return perfiles;
        }
    }
}
