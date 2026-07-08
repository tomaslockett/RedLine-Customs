namespace RedLine.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregadoTelefonoUsuario : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Factura",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        NumeroFactura = c.String(),
                        FechaEmision = c.DateTime(nullable: false),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MetodoPago = c.String(),
                        IVA = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Venta", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.Venta",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NumeroVenta = c.String(),
                        Fecha = c.DateTime(nullable: false),
                        IVA = c.Int(nullable: false),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AutoBase_ID = c.Int(),
                        AutoPersonalizado_ID = c.Int(),
                        Cliente_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AutoBase", t => t.AutoBase_ID)
                .ForeignKey("dbo.AutoPersonalizado", t => t.AutoPersonalizado_ID)
                .ForeignKey("dbo.Cliente", t => t.Cliente_ID)
                .Index(t => t.AutoBase_ID)
                .Index(t => t.AutoPersonalizado_ID)
                .Index(t => t.Cliente_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Factura", "ID", "dbo.Venta");
            DropForeignKey("dbo.Venta", "Cliente_ID", "dbo.Cliente");
            DropForeignKey("dbo.Venta", "AutoPersonalizado_ID", "dbo.AutoPersonalizado");
            DropForeignKey("dbo.Venta", "AutoBase_ID", "dbo.AutoBase");
            DropIndex("dbo.Venta", new[] { "Cliente_ID" });
            DropIndex("dbo.Venta", new[] { "AutoPersonalizado_ID" });
            DropIndex("dbo.Venta", new[] { "AutoBase_ID" });
            DropIndex("dbo.Factura", new[] { "ID" });
            DropTable("dbo.Venta");
            DropTable("dbo.Factura");
        }
    }
}
