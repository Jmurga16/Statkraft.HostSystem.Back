using Stakraft.HostSystem.Support.SoporteEnum;

namespace Stakraft.HostSystem.Support.SoporteUtil
{
    public static class LogMensaje
    {
        public static string getMensajeLog(Enums.EstadoArchivo estadoLog)
        {
            switch (estadoLog)
            {
                case Enums.EstadoArchivo.Registrado:
                    return "Archivo registrado con exito";
                case Enums.EstadoArchivo.Procesado:
                    return "Archivo Procesado con exito";
                case Enums.EstadoArchivo.Enviado:
                    return "Archivo Enviado con exito";
                case Enums.EstadoArchivo.Rechazado:
                    return "Archivo Rechazado";
                case Enums.EstadoArchivo.Reingresado:
                    return "Archivo Reingresado con exito";
                case Enums.EstadoArchivo.Reprocesado:
                    return "Archivo Reprocesado con exito";
                case Enums.EstadoArchivo.Atendido:
                    return "Archivo Atendido con exito";
                default:
                    return "";
            }
        }

        public static string getMensajeError(Enums.EstadoArchivo estadoFallido)
        {
            switch (estadoFallido)
            {
                case Enums.EstadoArchivo.Procesado:
                    return "Error al intentar procesar el archivo ";
                case Enums.EstadoArchivo.Enviado:
                    return "Error al intentar Enviar el archivo ";
                case Enums.EstadoArchivo.Reingresado:
                    return "Error al intentar Reenviar el archivo ";
                case Enums.EstadoArchivo.Rechazado:
                    return "Error al intentar registrar rechazo del archivo ";
                case Enums.EstadoArchivo.Reprocesado:
                    return "Error al intentar reprocesar el archivo ";
                default:
                    return "";
            }
        }
    }
}