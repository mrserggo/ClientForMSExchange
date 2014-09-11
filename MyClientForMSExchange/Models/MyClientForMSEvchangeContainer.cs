namespace MyClientForMSExchange.Models
{
    using System.Data.Entity;
    using Core.EntityFrameworkDAL.Entities;

    /// <summary>
    /// The my client for ms evchange container.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class MyClientForMSExchangeContainer : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyClientForMSExchangeContainer"/> class.
        /// </summary>
        public MyClientForMSExchangeContainer() : base("name=MyConnection")
        {
        }

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        public DbSet<Client> Clients { get; set; }

        /// <summary>
        /// Gets or sets the catalogs.
        /// </summary>
        public DbSet<Catalog> Catalogs { get; set; }

        /// <summary>
        /// Gets or sets the email items.
        /// </summary>
        public DbSet<EmailItem> EmailItems { get; set; }

        /// <summary>
        /// The on model creating.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // one-to-many 
            modelBuilder.Entity<EmailItem>().HasRequired<Client>(s => s.Client)
             .WithMany(s => s.EmailItems).HasForeignKey(s => s.ClientId);

            base.OnModelCreating(modelBuilder);
        }
    }
}