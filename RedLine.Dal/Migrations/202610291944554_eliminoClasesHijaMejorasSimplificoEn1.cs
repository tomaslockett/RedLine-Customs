namespace RedLine.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eliminoClasesHijaMejorasSimplificoEn1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mejora", "Detalle", c => c.String());
            DropColumn("dbo.Mejora", "Estilo");
            DropColumn("dbo.Mejora", "EsAjustable");
            DropColumn("dbo.Mejora", "CargaAerodinamica");
            DropColumn("dbo.Mejora", "Material");
            DropColumn("dbo.Mejora", "CantidadPiezas");
            DropColumn("dbo.Mejora", "RequierePintura");
            DropColumn("dbo.Mejora", "Rodado");
            DropColumn("dbo.Mejora", "Ancho");
            DropColumn("dbo.Mejora", "Terminacion");
            DropColumn("dbo.Mejora", "TipoAcabado");
            DropColumn("dbo.Mejora", "CodigoColor");
            DropColumn("dbo.Mejora", "EsVinilo");
            DropColumn("dbo.Mejora", "Tipo");
            DropColumn("dbo.Mejora", "ReduccionAltura");
            DropColumn("dbo.Mejora", "NivelesDureza");
            DropColumn("dbo.Mejora", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Mejora", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Mejora", "NivelesDureza", c => c.Int());
            AddColumn("dbo.Mejora", "ReduccionAltura", c => c.Double());
            AddColumn("dbo.Mejora", "Tipo", c => c.String());
            AddColumn("dbo.Mejora", "EsVinilo", c => c.Boolean());
            AddColumn("dbo.Mejora", "CodigoColor", c => c.String());
            AddColumn("dbo.Mejora", "TipoAcabado", c => c.String());
            AddColumn("dbo.Mejora", "Terminacion", c => c.String());
            AddColumn("dbo.Mejora", "Ancho", c => c.Double());
            AddColumn("dbo.Mejora", "Rodado", c => c.Int());
            AddColumn("dbo.Mejora", "RequierePintura", c => c.Boolean());
            AddColumn("dbo.Mejora", "CantidadPiezas", c => c.Int());
            AddColumn("dbo.Mejora", "Material", c => c.String());
            AddColumn("dbo.Mejora", "CargaAerodinamica", c => c.Double());
            AddColumn("dbo.Mejora", "EsAjustable", c => c.Boolean());
            AddColumn("dbo.Mejora", "Estilo", c => c.String());
            DropColumn("dbo.Mejora", "Detalle");
        }
    }
}
