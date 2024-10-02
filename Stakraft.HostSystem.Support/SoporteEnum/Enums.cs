namespace Stakraft.HostSystem.Support.SoporteEnum
{
    public class Enums
    {
        public enum EstadoRegistro
        {
            Inactivo = 0,
            Activo = 1
        }
        public enum EstadoBackup
        {
            Inactivo = 0,
            Activo = 1
        }
        public enum TipoBlobStorage
        {
            OriginalBcp = 1,
            ProcesadoBcp = 2,
            OriginalBbva = 3,
            ProcesadoBbva = 4
        }
        public enum EstadoCargaArchivo
        {
            Error = 1,
            Correcto = 2
        }
        public enum EstadoArchivo
        {
            Registrado = 1,
            Procesado = 2,
            Enviado = 3,
            Reenviado = 4,
            Reprocesado = 5,
            Rechazado = 6,
            Atendido = 7,
            Reingresado = 8,
            Erroneo = 9,
            Autorizado = 10,
            Recepcionado = 11,
        }
    }
}
