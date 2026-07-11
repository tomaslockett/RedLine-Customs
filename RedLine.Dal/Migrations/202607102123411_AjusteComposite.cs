namespace RedLine.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjusteComposite : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Familia", newName: "ComponentePermiso");
            CreateTable(
                "dbo.Perfil",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Permisos_Jerarquia",
                c => new
                    {
                        IdPadre = c.Int(nullable: false),
                        IdHijo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IdPadre, t.IdHijo })
                .ForeignKey("dbo.ComponentePermiso", t => t.IdPadre)
                .ForeignKey("dbo.ComponentePermiso", t => t.IdHijo)
                .Index(t => t.IdPadre)
                .Index(t => t.IdHijo);
            
            AddColumn("dbo.ComponentePermiso", "TipoComponente", c => c.String(nullable: false, maxLength: 128));
            DropTable("dbo.Permiso");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Permiso",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Permisos_Jerarquia", "IdHijo", "dbo.ComponentePermiso");
            DropForeignKey("dbo.Permisos_Jerarquia", "IdPadre", "dbo.ComponentePermiso");
            DropIndex("dbo.Permisos_Jerarquia", new[] { "IdHijo" });
            DropIndex("dbo.Permisos_Jerarquia", new[] { "IdPadre" });
            DropColumn("dbo.ComponentePermiso", "TipoComponente");
            DropTable("dbo.Permisos_Jerarquia");
            DropTable("dbo.Perfil");
            RenameTable(name: "dbo.ComponentePermiso", newName: "Familia");
        }
    }
}
