using Renci.SshNet;
using Stakraft.HostSystem.Service.ServiceDto.SFTP;
using System.Diagnostics;
using System.Text;

namespace Stakraft.HostSystem.Service.Service.Impl
{
    public class AuxFtpService : IAuxFtpService
    {
        private readonly static string SLASH = "/";
        private static Encoding encoding = Encoding.UTF8;
        public AuxFtpService()
        {
        }
        public bool TransferFile(string fileName, byte[] file, SftpDto connectionParams)
        {
            using var client = new SftpClient(connectionParams.IpServidor, connectionParams.PuertoServidor, connectionParams.UsuarioServidor, connectionParams.PasswordServidor);
            client.Connect();
            if (client.IsConnected)
            {
                Debug.WriteLine("Conectado al servidor sftp");

                using var ms = new MemoryStream(file);
                client.BufferSize = (uint)ms.Length;
                try
                {
                    client.UploadFile(ms, connectionParams.PathServidor + SLASH + fileName);
                    return true;

                }
                catch (Exception ex)
                {

                    Debug.WriteLine("Error al momento de cargar el archivo : " + ex.Message);
                    return false;
                }

            }
            else
            {
                Debug.WriteLine("No se pudo conectar al servidor sftp");
                return false;
            }
        }
        public string getContentFileOut(SftpDto connectionParams, string fileName)
        {
            string dato = null;
            using var sftp = new SftpClient(connectionParams.IpServidor, connectionParams.PuertoServidor, connectionParams.UsuarioServidor, connectionParams.PasswordServidor);
            sftp.Connect();
            StreamReader stream = null;
            try
            {
                var patFile = connectionParams.PathServidor + SLASH + fileName;
                var existe = sftp.Exists(patFile);
                if (!existe)
                {
                    return dato;
                }
                using var remoteFileStream = sftp.OpenRead(patFile);
                stream = new StreamReader(remoteFileStream);
                dato = stream.ReadToEnd();
            }
            finally
            {
                sftp.Disconnect();
                if (stream != null)
                    stream.Dispose();
            }
            return dato;
        }
        public string getContentFileOutBbva(SftpDto connectionParams, string fileName)
        {
            string dato = null;
            using var sftp = new SftpClient(connectionParams.IpServidor, connectionParams.PuertoServidor, connectionParams.UsuarioServidor, connectionParams.PasswordServidor);
            sftp.Connect();
            StreamReader stream = null;
            try
            {
                var listaArchivos = sftp.ListDirectory(connectionParams.PathServidor);
                var fullPath = "";
                foreach (var archivo in listaArchivos)
                {
                    if (archivo.FullName.Contains(fileName))
                    {
                        fullPath = archivo.FullName;
                        break;
                    }
                }
                if (fullPath != "")
                {
                    using var remoteFileStream = sftp.OpenRead(fullPath);
                    stream = new StreamReader(remoteFileStream);
                    dato = stream.ReadToEnd();
                }
            }
            finally
            {
                sftp.Disconnect();
                if (stream != null)
                    stream.Dispose();
            }
            return dato;
        }
    }
}
