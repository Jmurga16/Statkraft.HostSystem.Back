using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.Repository;
using Stakraft.HostSystem.Service.ServiceDto.ActiveDirectory;
using Stakraft.HostSystem.Service.ServiceDto.Usuario;
using Stakraft.HostSystem.Support.SoporteEnum;

namespace Stakraft.HostSystem.Service.Service.Impl
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IActiveDirectoryService activeDirectoryService;

        public UsuarioService(IUsuarioRepository usuarioRepository, IActiveDirectoryService activeDirectoryService)
        {
            _usuarioRepository = usuarioRepository;
            this.activeDirectoryService = activeDirectoryService;
        }

        public List<UsuarioDto> ListarUsuarios(bool activo)
        {
            var listUsuarios = _usuarioRepository.ListarUsuarios(activo).Select(usuario => new UsuarioDto
            {
                IdUsuario = usuario.IdUsuario,
                UserName = usuario.UserName,
                DisplayName = usuario.DisplayName,
                Email = usuario.Email,
                NombrePerfil = usuario.NombrePerfil,
                Activo = usuario.Activo,
                IdPerfil = usuario.IdPerfil,
                Estado = Enum.GetName(typeof(Enums.EstadoRegistro), Convert.ToInt32(usuario.Activo))
            }).ToList();

            return listUsuarios;
        }

        public UsuarioDto CrearUsuario(UsuarioDto usuario)
        {
            Usuarios usuarioEntity = new Usuarios
            {
                Activo = usuario.Activo,
                FechaCreacion = DateTime.Now,
                IdPerfil = usuario.IdPerfil,
                UsuarioCreacion = usuario.Usuario,
                UserName = usuario.UserName,
                DisplayName = usuario.DisplayName,
                Email = usuario.Email,
            };

            _usuarioRepository.CrearUsuario(usuarioEntity);

            usuario.IdUsuario = usuarioEntity.Id;
            usuario.Estado = Enum.GetName(typeof(Enums.EstadoRegistro), usuario.Activo);

            return usuario;
        }

        public UsuarioDto EditarUsuario(int idUsuario, UsuarioDto usuario)
        {
            Usuarios usuarioEntity = _usuarioRepository.ObtenerUsuario(idUsuario);
            usuarioEntity.UserName = usuario.UserName;
            usuarioEntity.DisplayName = usuario.DisplayName;
            usuarioEntity.Email = usuario.Email;
            usuarioEntity.Activo = usuario.Activo;
            usuarioEntity.FechaModificacion = DateTime.Now;
            usuarioEntity.UsuarioModificacion = usuario.Usuario;
            usuarioEntity.IdPerfil = usuario.IdPerfil;

            _usuarioRepository.EditarUsuario(usuarioEntity);
            usuario.Estado = Enum.GetName(typeof(Enums.EstadoRegistro), usuario.Activo);
            return usuario;
        }

        public bool CambiarEstado(int idUsuario)
        {
            Usuarios usuarioEntity = _usuarioRepository.ObtenerUsuario(idUsuario);
            usuarioEntity.Activo = !usuarioEntity.Activo;
            _usuarioRepository.EditarUsuario(usuarioEntity);

            return usuarioEntity.Activo;
        }

        public List<UserAd> ListarUsuariosAd()
        {
            var userlistaUsurAd = activeDirectoryService.ListarUsuarioAd();
            var listaUsuario = _usuarioRepository.ListarUsuarios(false);
            var usuarioAdFinal = new List<UserAd>();
            foreach (var userAd in userlistaUsurAd)
            {
                var exists = listaUsuario.Exists(usu => usu.UserName.Equals(userAd.UserName));
                if (!exists)
                {
                    usuarioAdFinal.Add(userAd);
                }
            }
            return usuarioAdFinal;
        }
    }
}
