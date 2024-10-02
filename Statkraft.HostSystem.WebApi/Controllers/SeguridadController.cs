using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stakraft.HostSystem.Service.Service;
using Stakraft.HostSystem.Service.ServiceDto.Seguridad;
using Stakraft.HostSystem.Support.soporte;
using Stakraft.HostSystem.Support.SoporteDto;

namespace Statkraft.HostSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeguridadController : Controller
    {
        private readonly ISeguridadService _seguridadService;

        public SeguridadController(ISeguridadService seguridadService)
        {
            _seguridadService = seguridadService;
        }
        [HttpPost]
        public Respuesta<LoginOut> Login(LoginIn loginIn)
        {
            var respuesta = new Respuesta<LoginOut>();
            var dato = _seguridadService.Login(loginIn);
            return respuesta.RespuestaCorrectaLogin(dato);
        }

        [HttpGet, AllowAnonymous]
        public Respuesta<LoginOut> VerificarUser()
        {
            var respuesta = new Respuesta<LoginOut>();
            var user = User?.Identity?.Name;
            if (user == null)
            {
                throw new StatkraftException("Usuario no registrado " + user);
            }
            var dato = _seguridadService.Login(user);
            return respuesta.RespuestaCorrectaLogin(dato);
        }
        [HttpPost("refreshToken")]
        public Respuesta<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var respuesta = new Respuesta<TokenDto>();
            var dato = _seguridadService.RefreshToken(tokenDto);
            return respuesta.RespuestaCorrecta(dato, "Refresh Token Completo");
        }
        [HttpPost("revokeToken")]
        public Respuesta<object> RevokarToken(TokenDto tokenDto)
        {
            var respuesta = new Respuesta<object>();
            _seguridadService.RevokarToken(tokenDto);
            return respuesta.RespuestaCorrectaLogin(null);
        }
    }
}
