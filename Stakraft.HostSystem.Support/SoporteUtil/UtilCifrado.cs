using System.Security.Cryptography;
using System.Text;

namespace Stakraft.HostSystem.Support.SoporteUtil
{
    public static class UtilCifrado
    {
        public static byte[] IV { get; set; } = Encoding.ASCII.GetBytes("Devjoker7.37hAES");
        public static byte[] Clave { get; set; } = Encoding.ASCII.GetBytes("AAECAwQFBgcICQoLDA0ODw==");

        public static string Encripta(string Cadena)
        {

            byte[] inputBytes = Encoding.ASCII.GetBytes(Cadena);
            byte[] encripted;
            RijndaelManaged cripto = new RijndaelManaged();
            using (MemoryStream ms = new MemoryStream(inputBytes.Length))
            {
                using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateEncryptor(Clave, IV), CryptoStreamMode.Write))
                {
                    objCryptoStream.Write(inputBytes, 0, inputBytes.Length);
                    objCryptoStream.FlushFinalBlock();
                    objCryptoStream.Close();
                }
                encripted = ms.ToArray();
            }
            return Convert.ToBase64String(encripted);
        }

        public static string Desencripta(string Cadena)
        {
            byte[] inputBytes = Convert.FromBase64String(Cadena);
            string textoLimpio = String.Empty;
            RijndaelManaged cripto = new RijndaelManaged();
            using (MemoryStream ms = new MemoryStream(inputBytes))
            {
                CryptoStream objCryptoStream = null;
                StreamReader sr = null;
                try
                {
                    objCryptoStream = new CryptoStream(ms, cripto.CreateDecryptor(Clave, IV), CryptoStreamMode.Read);
                    sr = new StreamReader(objCryptoStream, true);
                    textoLimpio = sr.ReadToEnd();
                }
                finally
                {
                    objCryptoStream?.Dispose();
                    sr?.Dispose();
                }
            }
            return textoLimpio;
        }
    }
}
