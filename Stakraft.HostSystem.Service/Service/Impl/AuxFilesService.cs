using Stakraft.HostSystem.Support.SoporteUtil;
using System.Text;

namespace Stakraft.HostSystem.Service.Service.Impl
{
    public class AuxFilesService : IAuxFilesService
    {
        public string ObtenerContenidoArchivo(byte[] file)
        {
            Encoding encoding = TextEncodingDetector.DetectTextByteArrayEncoding(file);
            if (encoding == null)
            {
                encoding = Encoding.GetEncoding("ISO-8859-1");
            }
            var original = String.Empty;
            Stream streamFile = new MemoryStream(file);
            using (StreamReader sr = new StreamReader(streamFile, encoding))
            {
                original = sr.ReadToEnd();
                encoding = sr.CurrentEncoding;
                sr.Close();
            }
            if (encoding == Encoding.UTF8)
                return original;

            byte[] encBytes = encoding.GetBytes(original);
            byte[] utf8Bytes = Encoding.Convert(encoding, Encoding.UTF8, encBytes);
            return Encoding.UTF8.GetString(utf8Bytes);
        }

        public byte[] ObtenerFileEscribir(string contenido)
        {
            MemoryStream stream = new MemoryStream();

            TextWriter tw = new StreamWriter(stream);
            tw.Write(contenido);
            tw.Flush();
            stream.Position = 0;
            byte[] file = stream.ToArray();

            return file;
        }


    }
}
