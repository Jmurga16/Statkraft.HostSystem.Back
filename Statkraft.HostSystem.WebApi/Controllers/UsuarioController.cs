using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stakraft.HostSystem.Service.Service;
using Stakraft.HostSystem.Service.ServiceDto.ActiveDirectory;
using Stakraft.HostSystem.Service.ServiceDto.Usuario;

namespace Statkraft.HostSystem.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("listar")]
        public ActionResult<List<UsuarioDto>> ListarUsuarios(string? dato)
        {
            var usuarios = _usuarioService.ListarUsuarios(false);
            return usuarios;
        }

        [HttpPost]
        public ActionResult<UsuarioDto> CrearUsuario(UsuarioDto usuarioDto)
        {
            var usuario = _usuarioService.CrearUsuario(usuarioDto);
            return usuario;
        }

        [HttpPut("{idusuario}")]
        public ActionResult<UsuarioDto> Editarusuario(int idusuario, [FromBody] UsuarioDto usuarioDto)
        {
            var usuario = _usuarioService.EditarUsuario(idusuario, usuarioDto);
            return usuario;
        }

        [HttpPut("estado/{idusuario}")]
        public ActionResult<bool> Estadousuario(int idusuario)
        {
            var usuario = _usuarioService.CambiarEstado(idusuario);
            return usuario;
        }

        [HttpPost("activos")]
        public ActionResult<List<UsuarioDto>> ListarusuarioesActivos()
        {
            var usuarioes = _usuarioService.ListarUsuarios(true);
            return usuarioes;
        }
        [HttpPost("listar/ad")]
        public ActionResult<List<UserAd>> ListarUsuariosAd()
        {
            var usuarioAd = _usuarioService.ListarUsuariosAd();
            return usuarioAd;
        }
    }
}
