using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stakraft.HostSystem.Repository.Entity
{
    public partial class Usuarios
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("id_perfil")]
        public int? IdPerfil { get; set; }
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
        [Column("password")]
        [StringLength(300)]
        public string? Password { get; set; }
        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; }
        [Column("user_name")]
        [StringLength(255)]
        public string UserName { get; set; }
        [Column("display_name")]
        [StringLength(255)]
        public string DisplayName { get; set; }

        [ForeignKey(nameof(IdPerfil))]
        [InverseProperty(nameof(Perfiles.Usuarios))]
        public virtual Perfiles IdPerfilNavigation { get; set; }
    }
}
