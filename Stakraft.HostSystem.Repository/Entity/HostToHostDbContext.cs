using Microsoft.EntityFrameworkCore;

namespace Stakraft.HostSystem.Repository.Entity
{
    public partial class HostToHostDbContext : DbContext
    {
        public HostToHostDbContext()
        {
        }

        public HostToHostDbContext(DbContextOptions<HostToHostDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Archivo> Archivo { get; set; }
        public virtual DbSet<Estados> Estados { get; set; }
        public virtual DbSet<LogArchivos> LogArchivos { get; set; }
        public virtual DbSet<Opciones> Opciones { get; set; }
        public virtual DbSet<Parametros> Parametros { get; set; }
        public virtual DbSet<PerfilOpcion> PerfilOpcion { get; set; }
        public virtual DbSet<Perfiles> Perfiles { get; set; }
        public virtual DbSet<ReemplazoCaracter> ReemplazoCaracter { get; set; }
        public virtual DbSet<TbBlobStorage> TbBlobStorage { get; set; }
        public virtual DbSet<TipoPlanilla> TipoPlanilla { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }
    }
}
