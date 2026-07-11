namespace RedLine.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActualizacionPerfilUsuario : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Usuario", "PerfilId", c => c.Int());
            CreateIndex("dbo.Usuario", "PerfilId");
            AddForeignKey("dbo.Usuario", "PerfilId", "dbo.Perfil", "Id");
            DropColumn("dbo.Usuario", "Rol");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Usuario", "Rol", c => c.String());
            DropForeignKey("dbo.Usuario", "PerfilId", "dbo.Perfil");
            DropIndex("dbo.Usuario", new[] { "PerfilId" });
            DropColumn("dbo.Usuario", "PerfilId");
        }
    }
}
