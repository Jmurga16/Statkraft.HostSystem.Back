using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stakraft.HostSystem.Repository.Entity
{
    public partial class Opciones
    {
        public Opciones()
        {
            PerfilOpcion = new HashSet<PerfilOpcion>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("nombre_opcion")]
        [StringLength(255)]
        public string NombreOpcion { get; set; }
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
        public string? UsuarioModificacion { get; set; }

        [InverseProperty("IdOpcionNavigation")]
        public virtual ICollection<PerfilOpcion> PerfilOpcion { get; set; }
    }
}
