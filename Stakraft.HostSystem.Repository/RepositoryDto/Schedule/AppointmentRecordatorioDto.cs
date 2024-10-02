namespace Stakraft.HostSystem.Repository.RepositoryDto.Schedule
{
    public class AppointmentRecordatorioDto
    {
        public string Paciente { get; set; }
        public string TelefonoPaciente { get; set; }
        public string TelefonoDoctor { get; set; }
        public string CorreoPaciente { get; set; }
        public DateTime? FechaCita { get; set; }
        public TimeSpan? HoraCita { get; set; }
        public string Doctor { get; set; }
        public string LinkTeleconsulta { get; set; }
    }
}
