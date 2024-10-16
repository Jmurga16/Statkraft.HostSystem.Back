using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stakraft.HostSystem.Repository.Entity
{
    public partial class Perfiles
    {
        public Perfiles()
        {
            PerfilOpcion = new HashSet<PerfilOpcion>();
            Usuarios = new HashSet<Usuarios>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("nombre_perfil")]
        [StringLength(255)]
        public string? NombrePerfil { get; set; }
        [Column("activo")]
        public bool Activo { get; set; }
        [Column("fecha_creacion", TypeName = "datetime")]
        public DateTime FechaCreacion { get; set; }
        [Required]
        [Column("usuario_creacion")]
        [StringLength(255)]
        public string UsuarioCreacion { get; set; } = "";
        [Column("fecha_modificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }
        [Column("usuario_modificacion")]
        [StringLength(255)]
        public string? UsuarioModificacion { get; set; }

        [InverseProperty("IdPerfilNavigation")]
        public virtual ICollection<PerfilOpcion> PerfilOpcion { get; set; }
        [InverseProperty("IdPerfilNavigation")]
        public virtual ICollection<Usuarios> Usuarios { get; set; }
    }
}
