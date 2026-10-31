namespace RedLine.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActualizacionGarageYFacturacion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AutoPersonalizado", "NombreExtra", c => c.String());
            AddColumn("dbo.AutoPersonalizado", "FechaCompra", c => c.DateTime());
            AddColumn("dbo.AutoPersonalizado", "NumeroVenta", c => c.String());
            AddColumn("dbo.AutoPersonalizado", "NumeroFactura", c => c.String());
            AddColumn("dbo.AutoPersonalizado", "MetodoPago", c => c.String());
            AddColumn("dbo.AutoPersonalizado", "TotalPagado", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Venta", "MetodoPago", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Venta", "MetodoPago");
            DropColumn("dbo.AutoPersonalizado", "TotalPagado");
            DropColumn("dbo.AutoPersonalizado", "MetodoPago");
            DropColumn("dbo.AutoPersonalizado", "NumeroFactura");
            DropColumn("dbo.AutoPersonalizado", "NumeroVenta");
            DropColumn("dbo.AutoPersonalizado", "FechaCompra");
            DropColumn("dbo.AutoPersonalizado", "NombreExtra");
        }
    }
}
