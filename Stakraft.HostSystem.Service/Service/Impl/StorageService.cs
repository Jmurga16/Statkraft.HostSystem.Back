using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Repository.Repository;
using Stakraft.HostSystem.Service.ServiceDto.Storage;
using Stakraft.HostSystem.Support.soporte;
using Stakraft.HostSystem.Support.SoporteEnum;

namespace Stakraft.HostSystem.Service.Service.Impl
{
    public class StorageService : IStorageService
    {
        private readonly IStorageRepository _storageRepository;
        private readonly IAuxBlobStorageService _auxBlobStorage;
        public StorageService(IStorageRepository storageRepository, IAuxBlobStorageService auxBlobStorage)
        {
            _storageRepository = storageRepository;
            _auxBlobStorage = auxBlobStorage;
        }
        public async Task GuardarStorage(StorageIn storageIn, string usuario)
        {
            var fechaActual = DateTime.Now;
            var tbStorage = new TbBlobStorage
            {
                Connection = storageIn.StringConnection,
                Container = storageIn.Container,
                Tipo = (short)storageIn.TipoStorage,
                Estado = (int)Enums.EstadoRegistro.Activo,
                UsuRegistro = usuario,
                FecRegistro = fechaActual
            };
            var fileTemporal = System.IO.File.ReadAllBytes("./ResourceDemo/no-image-found.jpg");
            var nombre = Guid.NewGuid().ToString() + ".png";
            bool guardo = false;
            try
            {
                await _auxBlobStorage.VerificarBlobStorageConeccion(fileTemporal, nombre, tbStorage);
                guardo = true;
            }
            catch (Exception e)
            {
                if (e is StatkraftException)
                {
                    throw new StatkraftException(e.Message);
                }
                else
                {
                    throw new StatkraftException("El connectionString es incorrecto");
                }
            }
            finally
            {
                if (guardo)
                {
                    _auxBlobStorage.EliminarArchivo(nombre, tbStorage);
                }
            }

            var tbStorageOld = _storageRepository.ObtenerTbBlobStorage(storageIn.TipoStorage, (int)Enums.EstadoRegistro.Activo);
            if (tbStorageOld != null)
            {
                tbStorageOld.Estado = (int)Enums.EstadoRegistro.Inactivo;
                tbStorageOld.FecModificacion = fechaActual;
                tbStorageOld.UsuModificacion = usuario;
                _storageRepository.InactivarStorage(tbStorageOld);
            }
            _storageRepository.GuardarTbBlobStorage(tbStorage);

        }
        public List<StorageOut> ListarStorage()
        {
            var listaTbStorageBackup = _storageRepository.ListarTbBlobStorage((int)Enums.EstadoRegistro.Activo);
            var ListaStorageOut = listaTbStorageBackup.Select(sto => new StorageOut
            {
                StringConnection = sto.Connection,
                Container = sto.Container,
                TipoStorage = sto.Tipo,
                NombreTipoStorage = Enum.GetName(typeof(Enums.TipoBlobStorage), sto.Tipo),
                Id = sto.Id
            }).OrderBy(sto => sto.TipoStorage).ToList();
            return ListaStorageOut;
        }
        public void InactivarStorage(int idStorage, string usuario)
        {
            var fechaActual = DateTime.Now;
            var tbStorage = _storageRepository.ObtenerTbBlobStorage(idStorage);
            tbStorage.Estado = (int)Enums.EstadoRegistro.Inactivo;
            tbStorage.FecModificacion = fechaActual;
            tbStorage.UsuModificacion = usuario;
            _storageRepository.InactivarStorage(tbStorage);
        }

    }
}