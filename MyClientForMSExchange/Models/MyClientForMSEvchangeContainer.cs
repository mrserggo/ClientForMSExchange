namespace MyClientForMSExchange.Models
{
    using System.Data.Entity;

    using MyClientForMSExchange.Models.Entities;

    /// <summary>
    /// The my client for ms evchange container.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class MyClientForMSExchangeContainer : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyClientForMSExchangeContainer"/> class.
        /// </summary>
        public MyClientForMSExchangeContainer()
            : base("name=MyConnection")
        {
        }

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        public DbSet<Client> Clients { get; set; }
    }
}