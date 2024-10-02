using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stakraft.HostSystem.Repository.Entity
{
    [Table("Log_Archivos")]
    public partial class LogArchivos
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("id_archivo")]
        public int IdArchivo { get; set; }
        [Column("id_estado")]
        public int IdEstado { get; set; }
        [Column("activo")]
        public bool Activo { get; set; }
        [Column("fecha_creacion", TypeName = "datetime")]
        public DateTime? FechaCreacion { get; set; }
        [Column("mensaje")]
        [StringLength(255)]
        public string Mensaje { get; set; }

        [ForeignKey(nameof(IdArchivo))]
        [InverseProperty(nameof(Archivo.LogArchivos))]
        public virtual Archivo IdArchivoNavigation { get; set; }
    }
}
