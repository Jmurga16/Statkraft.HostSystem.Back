using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stakraft.HostSystem.Repository.Entity
{
    [Table("Tipo_Planilla")]
    public partial class TipoPlanilla
    {
        public TipoPlanilla()
        {
            Archivo = new HashSet<Archivo>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("nombre")]
        [StringLength(255)]
        public string? Nombre { get; set; }
        [Column("usuario_creacion")]
        [StringLength(255)]
        public string? UsuarioCreacion { get; set; }
        [Column("fecha_creacion", TypeName = "datetime")]
        public DateTime? FechaCreacion { get; set; }
        [Column("usuario_modificacion")]
        [StringLength(255)]
        public string? UsuarioModificacion { get; set; }
        [Column("fecha_modificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }
        [Column("activo")]
        public bool? Activo { get; set; }
        [Column("prefijo")]
        [StringLength(5)]
        public string? Prefijo { get; set; }
        [Column("banco")]
        [StringLength(20)]
        public string? Banco { get; set; }

        [InverseProperty("IdTipoPlanillaNavigation")]
        public virtual ICollection<Archivo> Archivo { get; set; }
    }
}
