using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stakraft.HostSystem.Repository.Entity
{
    public partial class Parametros
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("nombre_parametro")]
        [StringLength(255)]
        public string? NombreParametro { get; set; }
        [Column("valor_parametro", TypeName = "text")]
        public string? ValorParametro { get; set; }
        [Column("fecha_creacion", TypeName = "datetime")]
        public DateTime? FechaCreacion { get; set; }
        [Column("usuario_creacion")]
        [StringLength(255)]
        public string? UsuarioCreacion { get; set; }
        [Column("fecha_modificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }
        [Column("usuario_modificacion")]
        [StringLength(255)]
        public string? UsuarioModificacion { get; set; }
    }
}
