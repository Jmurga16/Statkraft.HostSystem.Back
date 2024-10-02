using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.Repository;
using Stakraft.HostSystem.Service.ServiceDto.Perfil;
using Stakraft.HostSystem.Support.SoporteEnum;

namespace Stakraft.HostSystem.Service.Service.Impl
{
    public class PerfilService : IPerfilService
    {
        private readonly IPerfilRepository _perfilRepository;
        public PerfilService(IPerfilRepository perfilRepository)
        {
            _perfilRepository = perfilRepository;
        }

        public List<PerfilDto> ListarPerfiles(bool activo)
        {
            var listPerfiles = _perfilRepository.ListarPerfiles(activo).Select(perfil => new PerfilDto
            {
                IdPerfil = perfil.IdPerfil,
                NombrePerfil = perfil.NombrePerfil,
                Activo = perfil.Activo,
                Estado = Enum.GetName(typeof(Enums.EstadoRegistro), perfil.Activo)
            }).ToList();

            return listPerfiles;
        }

        public PerfilDto CrearPerfil(PerfilDto perfil)
        {
            Perfiles perfilEntity = new Perfiles
            {
                Activo = perfil.Activo,
                FechaCreacion = DateTime.Now,
                NombrePerfil = perfil.NombrePerfil,
                UsuarioCreacion = perfil.Usuario
            };

            _perfilRepository.CrearPerfil(perfilEntity);

            perfil.IdPerfil = perfilEntity.Id;

            return perfil;
        }

        public PerfilDto EditarPerfil(int idPerfil, PerfilDto perfil)
        {
            Perfiles perfilEntity = _perfilRepository.ObtenerPerfil(idPerfil);

            perfilEntity.NombrePerfil = perfil.NombrePerfil;
            perfilEntity.Activo = perfil.Activo;
            perfilEntity.FechaModificacion = DateTime.Now;
            perfilEntity.UsuarioModificacion = perfil.Usuario;
            _perfilRepository.EditarPerfil(perfilEntity);
            perfil.Estado = Enum.GetName(typeof(Enums.EstadoRegistro), perfil.Activo);
            return perfil;
        }

        public bool CambiarEstado(int idPerfil)
        {
            Perfiles perfilEntity = _perfilRepository.ObtenerPerfil(idPerfil);
            perfilEntity.Activo = !perfilEntity.Activo;
            _perfilRepository.EditarPerfil(perfilEntity);

            return perfilEntity.Activo;
        }
    }
}
