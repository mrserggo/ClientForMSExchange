using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyClientForMSExchange.Models
{
    using System.Data.Entity;

    using MyClientForMSExchange.Models.Entities;

    public class MyClientForMSEvchangeContainer : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyClientForMSEvchangeContainer"/> class.
        /// </summary>
        public MyClientForMSEvchangeContainer() : base("name=MyConnection") { }

        public DbSet<Client> Clients { get; set; }
    }
}