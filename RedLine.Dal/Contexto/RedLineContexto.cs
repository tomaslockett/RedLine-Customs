using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Servicios;
using RedLine.Servicios.Composite;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
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

        public DbSet<Idioma> Idiomas { get; set; }
        public DbSet<Etiqueta> Etiquetas { get; set; }
        public DbSet<Traduccion> Traducciones { get; set; }

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


            // PERFILES, PERMISOS Y FAMILIAS
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

            //IDIOMAS, ETIQUETAS Y TRADUCCIONES
            modelBuilder.Entity<Idioma>().ToTable("Idioma");
            modelBuilder.Entity<Idioma>().HasKey(i => i.ID);
            modelBuilder.Entity<Idioma>()
                .Property(i => i.Nombre)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<Idioma>().Ignore(i => i.Traducciones); 

            modelBuilder.Entity<Etiqueta>().ToTable("Etiqueta");
            modelBuilder.Entity<Etiqueta>().HasKey(e => e.ID);
            modelBuilder.Entity<Etiqueta>()
                .Property(e => e.Clave)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<Etiqueta>()
                .Property(e => e.Pagina)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Traduccion>().ToTable("Traduccion");
            modelBuilder.Entity<Traduccion>().HasKey(t => t.ID);

            // Relación Traduccion -> Idioma
            modelBuilder.Entity<Traduccion>()
                .HasRequired(t => t.Idioma)
                .WithMany(i => i.ListaTraducciones)
                .HasForeignKey(t => t.IdIdioma)
                .WillCascadeOnDelete(true);

            // Relación Traduccion -> Etiqueta
            modelBuilder.Entity<Traduccion>()
                .HasRequired(t => t.Etiqueta)
                .WithMany()
                .HasForeignKey(t => t.IdEtiqueta)
                .WillCascadeOnDelete(true);

            // Restricción Única Compuesta (IdIdioma + IdEtiqueta)
            modelBuilder.Entity<Traduccion>()
                .Property(t => t.IdIdioma)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UQ_Idioma_Etiqueta", 1) { IsUnique = true }));

            modelBuilder.Entity<Traduccion>()
                .Property(t => t.IdEtiqueta)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UQ_Idioma_Etiqueta", 2) { IsUnique = true }));

            //Confirmacion 
            base.OnModelCreating(modelBuilder);
        }
    }
}
