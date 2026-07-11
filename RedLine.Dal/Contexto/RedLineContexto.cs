using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Servicios;
using RedLine.Servicios.Composite;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace RedLine.Dal.Contexto
{
    public class RedLineContexto : DbContext
    {
        public RedLineContexto() : base("name=RedLineDB")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        // Definicion de Tablas
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Perfil> Perfil { get; set; }
        public DbSet<Familia> Familias { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<ComponentePermiso> Componentes { get; set; }
        public DbSet<DigitoVerificador> DigitoVerificador { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Factura> Facturas { get; set; }

        public DbSet<Evento> Bitacora { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Evento>().ToTable("Bitacora");

            modelBuilder.Entity<Venta>()
                .HasOptional(v => v.AutoBase)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Venta>()
                .HasOptional(v => v.AutoPersonalizado)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Venta>()
                .HasOptional(v => v.Factura)
                .WithRequired(f => f.Venta);

            modelBuilder.Entity<ComponentePermiso>()
                .HasMany(c => c.ComponentesHijos)
                .WithMany()
                .Map(m =>
                {
                     m.ToTable("Permisos_Jerarquia"); 
                     m.MapLeftKey("IdPadre");
                     m.MapRightKey("IdHijo");
                });

            modelBuilder.Entity<ComponentePermiso>()
                .Map<Familia>(m => m.Requires("TipoComponente").HasValue("Familia"))
                .Map<Permiso>(m => m.Requires("TipoComponente").HasValue("Permiso"));

            modelBuilder.Entity<Perfil>()
                .HasMany(p => p.PermisosRaiz) 
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("Perfil_Componente");
                    m.MapLeftKey("IdPerfil");
                    m.MapRightKey("IdComponente");
                });

            modelBuilder.Entity<ComponentePermiso>().ToTable("Componentes");

            modelBuilder.Entity<Usuario>()
                .HasOptional(u => u.Perfil) 
                .WithMany()
                .HasForeignKey(u => u.PerfilId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
