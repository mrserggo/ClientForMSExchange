namespace Core.EntityFrameworkDAL.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Core.EntityFrameworkDAL.Repositories.Interfaces;

    /// <summary>
    /// The client.
    /// </summary>
    public class Client : IEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [MaxLength(250)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the email items.
        /// </summary>
        /// <value>
        /// The email items.
        /// </value>
        public virtual ICollection<EmailItem> EmailItems { get; set; }

        #endregion
    }
}