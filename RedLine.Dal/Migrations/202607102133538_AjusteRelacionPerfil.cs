namespace RedLine.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjusteRelacionPerfil : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Perfil_Componente",
                c => new
                    {
                        IdPerfil = c.Int(nullable: false),
                        IdComponente = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IdPerfil, t.IdComponente })
                .ForeignKey("dbo.Perfil", t => t.IdPerfil, cascadeDelete: true)
                .ForeignKey("dbo.ComponentePermiso", t => t.IdComponente, cascadeDelete: true)
                .Index(t => t.IdPerfil)
                .Index(t => t.IdComponente);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Perfil_Componente", "IdComponente", "dbo.ComponentePermiso");
            DropForeignKey("dbo.Perfil_Componente", "IdPerfil", "dbo.Perfil");
            DropIndex("dbo.Perfil_Componente", new[] { "IdComponente" });
            DropIndex("dbo.Perfil_Componente", new[] { "IdPerfil" });
            DropTable("dbo.Perfil_Componente");
        }
    }
}
