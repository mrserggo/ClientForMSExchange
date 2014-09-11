namespace MyClientForMSExchange.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTablesforMSExchange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Catalogs",
                c => new
                    {
                        CatalogId = c.Long(nullable: false, identity: true),
                        CatalogName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CatalogId);
            
            CreateTable(
                "dbo.EmailItems",
                c => new
                    {
                        EmailItemId = c.Long(nullable: false, identity: true),
                        InternetMessageId = c.String(nullable: false),
                        Subject = c.String(nullable: false),
                        Body = c.String(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        CatalogId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.EmailItemId)
                .ForeignKey("dbo.Catalogs", t => t.CatalogId, cascadeDelete: true)
                .Index(t => t.CatalogId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailItems", "CatalogId", "dbo.Catalogs");
            DropIndex("dbo.EmailItems", new[] { "CatalogId" });
            DropTable("dbo.EmailItems");
            DropTable("dbo.Catalogs");
        }
    }
}
