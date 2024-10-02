using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.Repository;
using Stakraft.HostSystem.Service.ServiceDto.Parametros;
using System;

namespace Stakraft.HostSystem.Service.Service.Impl
{
    public class ParametrosService : IParametrosService
    {
        private readonly IParametrosRepository _parametrosRepository;
        public ParametrosService(IParametrosRepository parametrosRepository)
        {
            _parametrosRepository = parametrosRepository;
        }

        public ParametrosDto ObtenerParametros(string nombreParam)
        {
            var param = _parametrosRepository.ObtenerParametro(nombreParam);
            if (param == null)
            {
                return new ParametrosDto { NombreParametro = nombreParam };
            }
            else
            {
                return new ParametrosDto
                {
                    IdParametro = param.IdParametro,
                    NombreParametro = param.NombreParametro,
                    ValorParametro = param.ValorParametro
                };
            }
        }

        public ParametrosDto ActualizarParametros(ParametrosDto dto)
        {
            if (dto.IdParametro != null)
            {
                var paramEntity = _parametrosRepository.ObtenerEntity(dto.IdParametro.Value);

                paramEntity.ValorParametro = dto.ValorParametro;
                paramEntity.UsuarioModificacion = dto.Usuario;
                paramEntity.FechaModificacion = DateTime.Now;

                _parametrosRepository.ActualizarParametro(paramEntity);
            }
            else
            {
                var paramEntity = new Parametros
                {
                    ValorParametro = dto.ValorParametro,
                    NombreParametro = dto.NombreParametro,
                    UsuarioCreacion = dto.Usuario,
                    FechaCreacion = DateTime.Now
                };
                _parametrosRepository.GuardarParametro(paramEntity);
                dto.IdParametro = paramEntity.Id;
            }
            return dto;
        }
    }
}
