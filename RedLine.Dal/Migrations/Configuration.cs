namespace RedLine.Dal.Migrations
{
    using Redline.Be;
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

            context.AutosBase.AddOrUpdate(v => v.CodigoVehiculo,
        new AutoBase
        {
            // El ID no lo ponemos si es autoincremental (Identity) en la base de datos
            CodigoVehiculo = "POR-911-2026",
            Marca = "Porsche",
            Modelo = "911 Turbo S",
            Anio = 2026,
            PrecioBase = 230000.00m, // La 'm' indica que es un valor decimal
            Tipo = "Deportivo",
            Potencia = 650, // HP
            VelocidadMaxima = 330, // Km/h
            Aceleracion0a100 = 2.7m, // Segundos
            DescripcionGeneral = "Icono deportivo con tracción integral, rendimiento extremo y confort de primer nivel.",
            ImagenUrl = "Content/img/porsche.jfif" // Es recomendable guardarlo en una carpeta del proyecto
        }
    );

            //var familiaVendedor = new Familia { Nombre = "Vendedor" };

            //context.Familias.AddOrUpdate(f => f.Nombre, familiaVendedor);

            //context.SaveChanges();


        }
    }
}
