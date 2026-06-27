namespace RedLine.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAutoBaseImageToBinary : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AutoBase", "ImagenBinaria", c => c.Binary());
            DropColumn("dbo.AutoBase", "ImagenUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AutoBase", "ImagenUrl", c => c.String());
            DropColumn("dbo.AutoBase", "ImagenBinaria");
        }
    }
}
