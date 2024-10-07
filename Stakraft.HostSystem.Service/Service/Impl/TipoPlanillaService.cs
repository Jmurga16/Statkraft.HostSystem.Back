using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.Repository;
using Stakraft.HostSystem.Service.ServiceDto.TipoPlanilla;
using Stakraft.HostSystem.Support.soporte;
using Stakraft.HostSystem.Support.SoporteEnum;

namespace Stakraft.HostSystem.Service.Service.Impl
{
    public class TipoPlanillaService : ITipoPlanillaService
    {
        private readonly ITipoPlanillaRepository _tipoPlanillaRepository;
        public TipoPlanillaService(ITipoPlanillaRepository tipoPlanillaRepository)
        {
            _tipoPlanillaRepository = tipoPlanillaRepository;

        }

        public TipoPlanillaDto Actualizar(TipoPlanillaDto tipoPlanilla)
        {
            var existe = _tipoPlanillaRepository.Existe(tipoPlanilla.Nombre, tipoPlanilla.Id);
            if (existe)
            {
                throw new StatkraftException("El nombre " + tipoPlanilla.Nombre + " ya esta registrado");
            }
            var tbTipoPlantilla = new TipoPlanilla
            {
                Id = tipoPlanilla.Id.Value,
                Activo = tipoPlanilla.Activo,
                Nombre = tipoPlanilla.Nombre,
                FechaCreacion = DateTime.Now,
                Prefijo = tipoPlanilla.Prefijo,
                UsuarioCreacion = tipoPlanilla.Usuario,
                Banco = tipoPlanilla.Banco
            };
            _tipoPlanillaRepository.Actualizar(tbTipoPlantilla);
            tipoPlanilla.Estado = Enum.GetName(typeof(Enums.EstadoRegistro), Convert.ToInt32(tbTipoPlantilla.Activo));
            return tipoPlanilla;
        }

        public bool Estado(int idTipoPlanilla, string usuario)
        {
            return _tipoPlanillaRepository.Inactivar(idTipoPlanilla, usuario);
        }

        public TipoPlanillaDto Guardar(TipoPlanillaDto tipoPlanilla)
        {
            var existe = _tipoPlanillaRepository.Existe(tipoPlanilla.Nombre, null);
            if (existe)
            {
                throw new StatkraftException("El nombre " + tipoPlanilla.Nombre + " ya esta registrado");
            }
            var tbTipoPlantilla = new TipoPlanilla
            {
                Activo = tipoPlanilla.Activo,
                Banco = tipoPlanilla.Banco,
                Nombre = tipoPlanilla.Nombre,
                FechaCreacion = DateTime.Now,
                Prefijo = tipoPlanilla.Prefijo,
                UsuarioCreacion = tipoPlanilla.Usuario
            };
            _tipoPlanillaRepository.Guardar(tbTipoPlantilla);
            tipoPlanilla.Id = tbTipoPlantilla.Id;
            tipoPlanilla.Estado = Enum.GetName(typeof(Enums.EstadoRegistro), Convert.ToInt32(tbTipoPlantilla.Activo));
            return tipoPlanilla;
        }

        public List<TipoPlanillaDto> Listar(string banco)
        {
            var lista = _tipoPlanillaRepository.Listar(banco);
            return lista.Select(tip => new TipoPlanillaDto
            {
                Id = tip.Id,
                Activo = tip.Activo.Value,
                Estado = Enum.GetName(typeof(Enums.EstadoRegistro), Convert.ToInt32(tip.Activo)),
                Nombre = tip.Nombre,
                Prefijo = tip.Prefijo
            }
              ).ToList();
        }
    }
}
