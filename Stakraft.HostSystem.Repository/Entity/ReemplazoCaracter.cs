using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stakraft.HostSystem.Repository.Entity
{
    [Table("Reemplazo_Caracter")]
    public partial class ReemplazoCaracter
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("valor_original")]
        [StringLength(1)]
        public string ValorOriginal { get; set; }
        [Column("valor_reemplazo")]
        [StringLength(255)]
        public string ValorReemplazo { get; set; }
        [Column("activo")]
        public bool Activo { get; set; }
        [Column("fecha_creacion", TypeName = "datetime")]
        public DateTime? FechaCreacion { get; set; }
        [Column("usuario_creacion")]
        [StringLength(255)]
        public string UsuarioCreacion { get; set; }
        [Column("fecha_modificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }
        [Column("usuario_modificacion")]
        [StringLength(255)]
        public string UsuarioModificacion { get; set; }
    }
}
