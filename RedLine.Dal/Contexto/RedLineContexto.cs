using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Servicios.Composite;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DbSet<Familia> Familias { get; set; }   
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<DigitoVerificador> DigitoVerificador { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
