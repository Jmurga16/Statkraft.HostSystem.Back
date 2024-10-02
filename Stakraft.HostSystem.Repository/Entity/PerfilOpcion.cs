using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stakraft.HostSystem.Repository.Entity
{
    [Table("Perfil_Opcion")]
    public partial class PerfilOpcion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("id_perfil")]
        public int? IdPerfil { get; set; }
        [Column("id_opcion")]
        public int? IdOpcion { get; set; }
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

        [ForeignKey(nameof(IdOpcion))]
        [InverseProperty(nameof(Opciones.PerfilOpcion))]
        public virtual Opciones IdOpcionNavigation { get; set; }
        [ForeignKey(nameof(IdPerfil))]
        [InverseProperty(nameof(Perfiles.PerfilOpcion))]
        public virtual Perfiles IdPerfilNavigation { get; set; }
    }
}
