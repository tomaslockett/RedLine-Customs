namespace RedLine.Dal.Migrations
{
    using RedLine.Servicios.Composite;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<RedLine.Dal.Contexto.RedLineContexto>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(RedLine.Dal.Contexto.RedLineContexto context)
        {
            // ESTE METODO ES PARA QUE SE CARGUEN COSAS DE FORMA MANUAL EN LA BASE DE DATOS A LA HORA DE CREAR ALGO

            //var permisoVenta = new Permiso { Nombre = "Realizar Venta" };
            //var permisoStock = new Permiso { Nombre = "Cargar Stock" };
            //var permisoAdmin = new Permiso { Nombre = "Gestionar Usuarios" };

    
            //context.Permisos.AddOrUpdate(p => p.Nombre, permisoVenta, permisoStock, permisoAdmin);


            //var familiaVendedor = new Familia { Nombre = "Vendedor" };

            //context.Familias.AddOrUpdate(f => f.Nombre, familiaVendedor);

            //context.SaveChanges();
        }
    }
}
