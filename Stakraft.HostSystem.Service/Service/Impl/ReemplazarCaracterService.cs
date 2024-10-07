using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.Repository;
using Stakraft.HostSystem.Service.ServiceDto.ReemplazarCaracter;
using Stakraft.HostSystem.Support.SoporteEnum;

namespace Stakraft.HostSystem.Service.Service.Impl
{
    public class ReemplazarCaracterService : IReemplazarCaracterService
    {
        private readonly IReemplazaCaracterRepository _rCaracterRepository;
        public ReemplazarCaracterService(IReemplazaCaracterRepository rCaracterRepository)
        {
            _rCaracterRepository = rCaracterRepository;
        }
        public List<ReemplazarCaracterDto> ListarReemplazoCaracter(bool activo)
        {
            var listRCaracter = _rCaracterRepository.ListarCaracteres(activo).Select(rCaracter => new ReemplazarCaracterDto
            {
                IdRCaracter = rCaracter.IdRCaracter,
                ValorOriginal = rCaracter.ValorOriginal,
                ValorReemplazo = rCaracter.ValorReemplazo,
                Activo = rCaracter.Activo,
                Estado = Enum.GetName(typeof(Enums.EstadoRegistro), Convert.ToInt32(rCaracter.Activo))

            }).ToList();

            return listRCaracter;
        }
        public ReemplazarCaracterDto CrearReemplazoCaracter(ReemplazarCaracterDto rCaracter)
        {
            ReemplazoCaracter rCaracterEntity = new ReemplazoCaracter
            {
                Activo = rCaracter.Activo,
                FechaCreacion = DateTime.Now,
                ValorOriginal = rCaracter.ValorOriginal,
                ValorReemplazo = rCaracter.ValorReemplazo,
                UsuarioCreacion = rCaracter.Usuario
            };

            _rCaracterRepository.CrearReemplazoCaracter(rCaracterEntity);

            rCaracter.IdRCaracter = rCaracterEntity.Id;
            rCaracter.Estado = Enum.GetName(typeof(Enums.EstadoRegistro), Convert.ToInt32(rCaracter.Activo));
            return rCaracter;
        }
        public ReemplazarCaracterDto EditarReemplazoCaracter(int idRCaracter, ReemplazarCaracterDto rCaracter)
        {
            ReemplazoCaracter rCaracterEntity = _rCaracterRepository.ObtenerReemplazoCaracter(idRCaracter);

            rCaracterEntity.ValorOriginal = rCaracter.ValorOriginal;
            rCaracterEntity.ValorReemplazo = rCaracter.ValorReemplazo;
            rCaracterEntity.Activo = rCaracter.Activo;
            rCaracterEntity.FechaModificacion = DateTime.Now;
            rCaracterEntity.UsuarioModificacion = rCaracter.Usuario;

            _rCaracterRepository.EditarReemplazoCaracter(rCaracterEntity);
            rCaracter.Estado = Enum.GetName(typeof(Enums.EstadoRegistro), Convert.ToInt32(rCaracter.Activo));
            return rCaracter;
        }
        public bool CambiarEstado(int idRCaracter)
        {
            ReemplazoCaracter rCaracterEntity = _rCaracterRepository.ObtenerReemplazoCaracter(idRCaracter);
            rCaracterEntity.Activo = !rCaracterEntity.Activo;
            _rCaracterRepository.EditarReemplazoCaracter(rCaracterEntity);

            return rCaracterEntity.Activo;
        }
    }
}
