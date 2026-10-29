namespace RedLine.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregadoCampoIdiomaUsuario : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Usuario", "Idioma", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Usuario", "Idioma");
        }
    }
}
