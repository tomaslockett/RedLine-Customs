namespace RedLine.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregarTablasTraducciones : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Etiqueta",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Clave = c.String(nullable: false, maxLength: 100),
                        Pagina = c.String(nullable: false, maxLength: 100),
                        Descripcion = c.String(),
                        EsMensajeAlerta = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Idioma",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 50),
                        EsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Traduccion",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IdIdioma = c.Int(nullable: false),
                        IdEtiqueta = c.Int(nullable: false),
                        Texto = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Etiqueta", t => t.IdEtiqueta, cascadeDelete: true)
                .ForeignKey("dbo.Idioma", t => t.IdIdioma, cascadeDelete: true)
                .Index(t => new { t.IdIdioma, t.IdEtiqueta }, unique: true, name: "UQ_Idioma_Etiqueta");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Traduccion", "IdIdioma", "dbo.Idioma");
            DropForeignKey("dbo.Traduccion", "IdEtiqueta", "dbo.Etiqueta");
            DropIndex("dbo.Traduccion", "UQ_Idioma_Etiqueta");
            DropTable("dbo.Traduccion");
            DropTable("dbo.Idioma");
            DropTable("dbo.Etiqueta");
        }
    }
}
