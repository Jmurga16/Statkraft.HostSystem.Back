using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stakraft.HostSystem.Repository.Entity
{
    public partial class Estados
    {
        public Estados()
        {
            Archivo = new HashSet<Archivo>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_estado")]
        [StringLength(255)]
        public string? NombreEstado { get; set; }

        [InverseProperty("IdEstadoNavigation")]
        public virtual ICollection<Archivo> Archivo { get; set; }
    }
}
