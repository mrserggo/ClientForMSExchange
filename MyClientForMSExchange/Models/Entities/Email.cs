using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyClientForMSExchange.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    using MyClientForMSExchange.Repositories.Interfaces;

    public class Email : IEntity
    {
        #region Properties

        public Int64 Id { get; set; }

        [Required]
        [MaxLength(50)]
        public String EmailOwner { get; set; }

        [Required]
        [MaxLength(250)]
        public String Subject { get; set; }

        [Required]
        [MaxLength(10000)]
        public String Body { get; set; }

        #endregion
    }
}