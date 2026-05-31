namespace RedLine.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bitacora",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Usuario = c.String(),
                        Fecha = c.DateTime(nullable: false),
                        Modulo = c.String(),
                        Actividad = c.String(),
                        Criticidad = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Cliente",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DNI = c.String(),
                        Nombre = c.String(),
                        Apellido = c.String(),
                        Email = c.String(),
                        Contraseña = c.String(),
                        Telefono = c.String(),
                        Direccion = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AutoPersonalizado",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DNI_Cliente = c.String(),
                        AuBase_ID = c.Int(),
                        Cliente_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AutoBase", t => t.AuBase_ID)
                .ForeignKey("dbo.Cliente", t => t.Cliente_ID)
                .Index(t => t.AuBase_ID)
                .Index(t => t.Cliente_ID);
            
            CreateTable(
                "dbo.AutoBase",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CodigoVehiculo = c.String(),
                        Marca = c.String(),
                        Modelo = c.String(),
                        Anio = c.Int(nullable: false),
                        PrecioBase = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tipo = c.String(),
                        Potencia = c.Int(nullable: false),
                        VelocidadMaxima = c.Int(nullable: false),
                        Aceleracion0a100 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DescripcionGeneral = c.String(),
                        ImagenUrl = c.String(),
                        Stock = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Mejora",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Precio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Stock = c.Int(nullable: false),
                        Categoria = c.String(),
                        Estilo = c.String(),
                        EsAjustable = c.Boolean(),
                        CargaAerodinamica = c.Double(),
                        Material = c.String(),
                        CantidadPiezas = c.Int(),
                        RequierePintura = c.Boolean(),
                        Rodado = c.Int(),
                        Ancho = c.Double(),
                        Terminacion = c.String(),
                        TipoAcabado = c.String(),
                        CodigoColor = c.String(),
                        EsVinilo = c.Boolean(),
                        Tipo = c.String(),
                        ReduccionAltura = c.Double(),
                        NivelesDureza = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        AutoPersonalizado_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AutoPersonalizado", t => t.AutoPersonalizado_ID)
                .Index(t => t.AutoPersonalizado_ID);
            
            CreateTable(
                "dbo.DigitoVerificador",
                c => new
                    {
                        NombreTabla = c.String(nullable: false, maxLength: 128),
                        DVH = c.String(),
                        DVV = c.String(),
                    })
                .PrimaryKey(t => t.NombreTabla);
            
            CreateTable(
                "dbo.Familia",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Permiso",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DNI = c.String(),
                        Nombre = c.String(),
                        Apellido = c.String(),
                        Email = c.String(),
                        Contraseña = c.String(),
                        Rol = c.String(),
                        Intentos = c.Int(nullable: false),
                        Bloqueado = c.Boolean(nullable: false),
                        Activo = c.Boolean(nullable: false),
                        UltimoIntento = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AutoPersonalizado", "Cliente_ID", "dbo.Cliente");
            DropForeignKey("dbo.Mejora", "AutoPersonalizado_ID", "dbo.AutoPersonalizado");
            DropForeignKey("dbo.AutoPersonalizado", "AuBase_ID", "dbo.AutoBase");
            DropIndex("dbo.Mejora", new[] { "AutoPersonalizado_ID" });
            DropIndex("dbo.AutoPersonalizado", new[] { "Cliente_ID" });
            DropIndex("dbo.AutoPersonalizado", new[] { "AuBase_ID" });
            DropTable("dbo.Usuario");
            DropTable("dbo.Permiso");
            DropTable("dbo.Familia");
            DropTable("dbo.DigitoVerificador");
            DropTable("dbo.Mejora");
            DropTable("dbo.AutoBase");
            DropTable("dbo.AutoPersonalizado");
            DropTable("dbo.Cliente");
            DropTable("dbo.Bitacora");
        }
    }
}
