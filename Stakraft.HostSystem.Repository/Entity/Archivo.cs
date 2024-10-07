using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stakraft.HostSystem.Repository.Entity
{
    public partial class Archivo
    {
        public Archivo()
        {
            LogArchivos = new HashSet<LogArchivos>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("nombre_archivo")]
        [StringLength(255)]
        public string NombreArchivo { get; set; }
        [Column("id_tipo_planilla")]
        public int? IdTipoPlanilla { get; set; }
        [Column("id_estado")]
        public int? IdEstado { get; set; }
        [Column("fecha_creacion", TypeName = "datetime")]
        public DateTime? FechaCreacion { get; set; }
        [Column("usuario_creacion")]
        [StringLength(255)]
        public string UsuarioCreacion { get; set; }
        [Column("fecha_modificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }
        [Column("usuario_modificacion")]
        [StringLength(255)]
        public string? UsuarioModificacion { get; set; }
        [Column("blo_sto_id_original")]
        public int? BloStoIdOriginal { get; set; }
        [Column("blo_sto_id_procesado")]
        public int? BloStoIdProcesado { get; set; }
        [Column("men_error", TypeName = "text")]
        public string MenError { get; set; }
        [Column("nombre_archivo_procesado")]
        [StringLength(255)]
        public string NombreArchivoProcesado { get; set; }
        [Column("banco")]
        [StringLength(20)]
        public string Banco { get; set; }

        [ForeignKey(nameof(BloStoIdOriginal))]
        [InverseProperty(nameof(TbBlobStorage.ArchivoBloStoIdOriginalNavigation))]
        public virtual TbBlobStorage BloStoIdOriginalNavigation { get; set; }
        [ForeignKey(nameof(BloStoIdProcesado))]
        [InverseProperty(nameof(TbBlobStorage.ArchivoBloStoIdProcesadoNavigation))]
        public virtual TbBlobStorage BloStoIdProcesadoNavigation { get; set; }
        [ForeignKey(nameof(IdEstado))]
        [InverseProperty(nameof(Estados.Archivo))]
        public virtual Estados IdEstadoNavigation { get; set; }
        [ForeignKey(nameof(IdTipoPlanilla))]
        [InverseProperty(nameof(TipoPlanilla.Archivo))]
        public virtual TipoPlanilla IdTipoPlanillaNavigation { get; set; }
        [InverseProperty("IdArchivoNavigation")]
        public virtual ICollection<LogArchivos> LogArchivos { get; set; }
    }

}
