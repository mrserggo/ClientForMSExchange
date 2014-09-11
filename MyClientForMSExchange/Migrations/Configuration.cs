namespace MyClientForMSExchange.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using Core.EntityFrameworkDAL.Entities;

    internal sealed class Configuration : DbMigrationsConfiguration<MyClientForMSExchange.Models.MyClientForMSExchangeContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        /// <summary>
        /// The seed.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Seed(Models.MyClientForMSExchangeContainer context)
        {
            context.Catalogs.AddOrUpdate(
                c => c.CatalogName,
                new Catalog() { CatalogName = "Inbox" },
                new Catalog() { CatalogName = "SentItems" },
                new Catalog() { CatalogName = "DeletedItems" },
                new Catalog() { CatalogName = "Drafts" });
        }
    }
}
