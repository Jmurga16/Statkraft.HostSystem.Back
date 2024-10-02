using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.Repository;
using Stakraft.HostSystem.Service.ServiceDto.Parametros;
using Stakraft.HostSystem.Service.ServiceDto.Seguridad;
using Stakraft.HostSystem.Support.soporte;
using Stakraft.HostSystem.Support.SoporteUtil;
using Stakraft.HostSystem.Support.Token;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Stakraft.HostSystem.Service.Service.Impl
{
    public class SeguridadService : ISeguridadService
    {
        private readonly ISeguridadRepository _seguridadRepository;
        private readonly IActiveDirectoryService activeDirectoryService;
        private readonly ITokenGenerador _tokenGenerador;
        public SeguridadService(ISeguridadRepository seguridadRepository,
            ITokenGenerador tokenGenerador, IActiveDirectoryService activeDirectoryService)
        {
            _seguridadRepository = seguridadRepository;
            _tokenGenerador = tokenGenerador;
            this.activeDirectoryService = activeDirectoryService;
        }

        public LoginOut Login(LoginIn seguridadIn)
        {
            Usuarios user = _seguridadRepository.BuscarUsuario(seguridadIn.Usuario, UtilCifrado.Encripta(seguridadIn.Contrasenia));
            if (user == null)
            {
                AdConfiguracionDto sftpParams = activeDirectoryService.ObtenerConfiguracionActiveDirectory();
                PrincipalContext context = new PrincipalContext(ContextType.Domain, sftpParams.Ip, sftpParams.Ldap);
                var userValid = context.ValidateCredentials(seguridadIn.Usuario, seguridadIn.Contrasenia);
                if (!userValid)
                {
                    throw new StatkraftException("El usuario no esta registrado en el directorio");
                }
                user = _seguridadRepository.BuscarUsuario(seguridadIn.Usuario);
                if (user == null)
                {
                    throw new StatkraftException("El usuario no esta registrado en el sistema");
                }
            }
            LoginOut loginOut = ObtenerUsuarioLogeado(user);
            return loginOut;
        }

        public LoginOut Login(string usuarioAd)
        {
            var usuarioAdEncontrado = activeDirectoryService.getUsuarioAd(usuarioAd);
            if (usuarioAdEncontrado == null)
            {
                throw new StatkraftException("El usuario " + usuarioAdEncontrado + "  no esta registrado en el directorio");
            }
            Usuarios user = _seguridadRepository.BuscarUsuario(usuarioAdEncontrado);
            if (user == null)
            {
                throw new StatkraftException("El usuario " + usuarioAd + " no esta registrado en el sistema");
            }
            LoginOut loginOut = ObtenerUsuarioLogeado(user);
            return loginOut;
        }

        private LoginOut ObtenerUsuarioLogeado(Usuarios user)
        {
            var claims = new[] {

                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.IdPerfilNavigation.NombrePerfil),
            };
            var token = _tokenGenerador.GenerateAccessToken(claims);
            var refreshToken = _tokenGenerador.GenerateRefreshToken();
            _tokenGenerador.SaveUserRefresToken(user.UserName, refreshToken);
            var loginOut = new LoginOut
            {
                Usuario = new LoginOut.LoginUsuarioOut
                {
                    IdPerfil = user.IdPerfil,
                    IdUsuario = user.Id,
                    Usuario = user.DisplayName,
                    Perfil = user.IdPerfilNavigation.NombrePerfil,
                    FechaLogin = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                },
                Token = new TokenDto { Token = token, RefreshToken = refreshToken }
            };
            return loginOut;
        }

        public TokenDto RefreshToken(TokenDto tokenDto)
        {
            string accessToken = tokenDto.Token;
            string refreshToken = tokenDto.RefreshToken; //usado para comprabar si el refresk tokene sta registrado en el usuario actualmente no regista
            var principal = _tokenGenerador.GetPrincipalFromExpiredToken(accessToken);
            var cuentaUsuario = principal.Identity.Name;
            var user = _seguridadRepository.BuscarUsuario(cuentaUsuario);
            var refresTokenActive = _tokenGenerador.RefreshTokenActive(cuentaUsuario, refreshToken);
            if (user == null || !refresTokenActive)
            {
                throw new StatkraftException("Token invalido");
            }
            var newAccessToken = _tokenGenerador.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenGenerador.GenerateRefreshToken();
            _tokenGenerador.SaveUserRefresToken(cuentaUsuario, newRefreshToken);
            return new TokenDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public void RevokarToken(TokenDto tokenDto)
        {
            var principal = _tokenGenerador.GetPrincipalFromExpiredToken(tokenDto.Token);
            var cuentaUsuario = principal.Identity.Name;
            var user = _seguridadRepository.BuscarUsuario(cuentaUsuario);
            if (user == null)
            {
                throw new StatkraftException("Token invalido");
            }
            _tokenGenerador.RemoveUserRefreshToken(user.UserName);
        }

    }
}
