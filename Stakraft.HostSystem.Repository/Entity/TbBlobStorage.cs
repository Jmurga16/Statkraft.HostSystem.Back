using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stakraft.HostSystem.Repository.Entity
{
    [Table("Tb_Blob_Storage")]
    public partial class TbBlobStorage
    {
        public TbBlobStorage()
        {
            ArchivoBloStoIdOriginalNavigation = new HashSet<Archivo>();
            ArchivoBloStoIdProcesadoNavigation = new HashSet<Archivo>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("connection", TypeName = "text")]
        public string Connection { get; set; }
        [Column("fec_registro", TypeName = "datetime")]
        public DateTime FecRegistro { get; set; }
        [Required]
        [Column("usu_registro")]
        [StringLength(255)]
        public string UsuRegistro { get; set; }
        [Column("tipo")]
        public int Tipo { get; set; }
        [Column("fec_modificacion", TypeName = "date")]
        public DateTime? FecModificacion { get; set; }
        [Column("usu_modificacion")]
        [StringLength(255)]
        public string? UsuModificacion { get; set; }
        [Column("estado")]
        public int Estado { get; set; }
        [Column("container")]
        [StringLength(255)]
        public string Container { get; set; }

        [InverseProperty(nameof(Archivo.BloStoIdOriginalNavigation))]
        public virtual ICollection<Archivo> ArchivoBloStoIdOriginalNavigation { get; set; }
        [InverseProperty(nameof(Archivo.BloStoIdProcesadoNavigation))]
        public virtual ICollection<Archivo> ArchivoBloStoIdProcesadoNavigation { get; set; }
    }
}
