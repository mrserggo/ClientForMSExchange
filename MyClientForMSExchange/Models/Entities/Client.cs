using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyClientForMSExchange.Repositories.Interfaces;

namespace MyClientForMSExchange.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    using Core.EntityFrameworkDAL.Repositories.Interfaces;

    public class Client : IEntity
    {
        #region Properties

        public Int64 Id { get; set; }

        [Required]
        [MaxLength(50)]
        public String Email { get; set; }

        [Required]
        [MaxLength(250)]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        #endregion
    }
}