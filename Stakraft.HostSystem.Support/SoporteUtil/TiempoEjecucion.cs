namespace Stakraft.HostSystem.Support.SoporteUtil
{
    public static class TiempoEjecucion
    {
        public static DateTime fechaEjecucionProcesoBcp { get; set; } = DateTime.Now;
        public static DateTime fechaEjecucionEnvioBancoInBcp { get; set; } = DateTime.Now;
        public static DateTime fechaEjecucionLecturaBancoOutBcp { get; set; } = DateTime.Now;
        public static DateTime fechaEjecucionProcesoBbva { get; set; } = DateTime.Now;
        public static DateTime fechaEjecucionEnvioBancoInBbva { get; set; } = DateTime.Now;
        public static DateTime fechaEjecucionLecturaBancoOutBbva { get; set; } = DateTime.Now;
        public static Dictionary<string, int> secuenciaPlanillas { get; set; } = new Dictionary<string, int>();

    }
}
