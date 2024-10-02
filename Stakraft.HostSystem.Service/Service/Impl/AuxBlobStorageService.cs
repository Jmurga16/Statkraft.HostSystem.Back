using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Stakraft.HostSystem.Repository.Entity;
using Stakraft.HostSystem.Service.ServiceDto.Resource;
using Stakraft.HostSystem.Support.soporte;
using Stakraft.HostSystem.Support.SoporteUtil;

namespace Stakraft.HostSystem.Service.Service.Impl
{
    public class AuxBlobStorageService : IAuxBlobStorageService
    {

        public async Task<ResourceStorage> ObtenerArchivoBase64(string nombreArchivo, TbBlobStorage tbBlobStorage)
        {
            var fileBytes = await ObtenerFileByte(nombreArchivo, tbBlobStorage);
            fileBytes.base64 = Convert.ToBase64String(fileBytes.FileBytes);
            return fileBytes;
        }

        public async Task<ResourceStorage> ObtenerArchivo(string nombreArchivo, TbBlobStorage tbBlobStorage)
        {
            var resourceAdjuntos = await ObtenerFileByte(nombreArchivo, tbBlobStorage);
            return resourceAdjuntos;
        }

        private async Task<ResourceStorage> ObtenerFileByte(string nombreArchivo, TbBlobStorage tbBlobStorage)
        {
            var resource = new ResourceStorage();
            var archivo = System.Uri.UnescapeDataString(nombreArchivo);
            BlobContainerClient containerClient = getBlobContainer(tbBlobStorage);
            BlobClient blobClient = containerClient.GetBlobClient(archivo);
            BlobDownloadInfo file = await blobClient.DownloadAsync();
            var indexInicial = nombreArchivo.IndexOf("/") + 1;
            var nombre = nombreArchivo.Substring(indexInicial);
            using (MemoryStream ms = new MemoryStream())
            {
                await file.Content.CopyToAsync(ms);
                resource.FileBytes = ms.ToArray();
                resource.ContentType = file.ContentType;
                resource.Name = nombre;
            }

            return resource;
        }

        public async Task GuardarArchivo(string adjuntoBase64, string nombre, TbBlobStorage tbBlobStorage)
        {
            var bytes = Convert.FromBase64String(adjuntoBase64);
            BlobContainerClient blobContainerClient = getBlobContainer(tbBlobStorage);
            blobContainerClient.CreateIfNotExists();
            BlobClient blobClient = blobContainerClient.GetBlobClient(nombre);
            Stream uploadFileStream = new MemoryStream(bytes);
            await blobClient.UploadAsync(uploadFileStream);
            uploadFileStream.Close();
        }
        public async Task GuardarArchivo(IFormFile file, string nombre, TbBlobStorage tbBlobStorage)
        {
            BlobContainerClient blobContainerClient = getBlobContainer(tbBlobStorage);
            blobContainerClient.CreateIfNotExists();
            BlobClient blobClient = blobContainerClient.GetBlobClient(nombre);
            Stream uploadFileStream = file.OpenReadStream();
            await blobClient.UploadAsync(uploadFileStream, new BlobHttpHeaders { ContentType = file.ContentType });
            uploadFileStream.Close();
        }

        public async Task GuardarArchivo(byte[] file, string nombre, TbBlobStorage tbBlobStorage)
        {
            BlobContainerClient blobContainerClient = getBlobContainer(tbBlobStorage);
            blobContainerClient.CreateIfNotExists();
            BlobClient blobClient = blobContainerClient.GetBlobClient(nombre);
            var uploadFileStream = new MemoryStream(file, true);
            await blobClient.UploadAsync(uploadFileStream, new BlobHttpHeaders { ContentType = "text/plain" });
            uploadFileStream.Close();
        }

        public void EliminarArchivo(string nombre, TbBlobStorage tbBlobStorage)
        {
            BlobContainerClient blobContainerClient = getBlobContainer(tbBlobStorage);
            BlobClient blobClient = blobContainerClient.GetBlobClient(nombre);
            blobClient.DeleteIfExists();
        }

        public async Task VerificarBlobStorageConeccion(byte[] file, string nombre, TbBlobStorage tbBlobStorage)
        {
            BlobContainerClient blobContainerClient = getBlobContainer(tbBlobStorage);
            bool existeContenedor;
            try
            {
                existeContenedor = await blobContainerClient.ExistsAsync();
            }
            catch (Exception)
            {
                throw new StatkraftException("El connectionString es incorrecto");
            }
            if (!existeContenedor)
            {
                throw new StatkraftException("El contenedor " + GetContainer(tbBlobStorage) + " no existe.");
            }
            BlobClient blobClient = blobContainerClient.GetBlobClient(nombre);
            var uploadFileStream = new MemoryStream(file, true);
            try
            {
                await blobClient.UploadAsync(uploadFileStream, overwrite: true);
                uploadFileStream.Close();
            }
            catch (Exception)
            {
                uploadFileStream.Close();
                throw new StatkraftException("Sin permisos de escritura. No se pudo guardar el archivo de prueba en el contenedor.");
            }
        }

        private BlobContainerClient getBlobContainer(TbBlobStorage tbBlobStorage)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(GetConnection(tbBlobStorage));
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(GetContainer(tbBlobStorage));
            return containerClient;
        }
        private string GetContainer(TbBlobStorage tbBlobStorage)
        {
            return UtilCifrado.Desencripta(tbBlobStorage.Container);
        }
        private string GetConnection(TbBlobStorage tbBlobStorage)
        {
            return UtilCifrado.Desencripta(tbBlobStorage.Connection);
        }

    }
}
