namespace RedLine.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjusteCompositeFinal : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ComponentePermiso", newName: "Componentes");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Componentes", newName: "ComponentePermiso");
        }
    }
}
