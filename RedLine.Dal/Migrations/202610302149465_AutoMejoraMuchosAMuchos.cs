namespace RedLine.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AutoMejoraMuchosAMuchos : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Mejora", "AutoPersonalizado_ID", "dbo.AutoPersonalizado");
            DropIndex("dbo.Mejora", new[] { "AutoPersonalizado_ID" });
            CreateTable(
                "dbo.Auto_Mejora",
                c => new
                    {
                        ID_Auto = c.Int(nullable: false),
                        ID_Mejora = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ID_Auto, t.ID_Mejora })
                .ForeignKey("dbo.AutoPersonalizado", t => t.ID_Auto, cascadeDelete: true)
                .ForeignKey("dbo.Mejora", t => t.ID_Mejora, cascadeDelete: true)
                .Index(t => t.ID_Auto)
                .Index(t => t.ID_Mejora);
            
            DropColumn("dbo.Mejora", "AutoPersonalizado_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Mejora", "AutoPersonalizado_ID", c => c.Int());
            DropForeignKey("dbo.Auto_Mejora", "ID_Mejora", "dbo.Mejora");
            DropForeignKey("dbo.Auto_Mejora", "ID_Auto", "dbo.AutoPersonalizado");
            DropIndex("dbo.Auto_Mejora", new[] { "ID_Mejora" });
            DropIndex("dbo.Auto_Mejora", new[] { "ID_Auto" });
            DropTable("dbo.Auto_Mejora");
            CreateIndex("dbo.Mejora", "AutoPersonalizado_ID");
            AddForeignKey("dbo.Mejora", "AutoPersonalizado_ID", "dbo.AutoPersonalizado", "ID");
        }
    }
}
