namespace MyClientForMSExchange.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedReferencesFromClienttoEmailItemOnetoMany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailItems", "ClientId", c => c.Long(nullable: false));
            CreateIndex("dbo.EmailItems", "ClientId");
            AddForeignKey("dbo.EmailItems", "ClientId", "dbo.Clients", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailItems", "ClientId", "dbo.Clients");
            DropIndex("dbo.EmailItems", new[] { "ClientId" });
            DropColumn("dbo.EmailItems", "ClientId");
        }
    }
}
