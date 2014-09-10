namespace Core.EntityFrameworkDAL.Entities
{
    using System.ComponentModel.DataAnnotations;

    using Core.EntityFrameworkDAL.Repositories.Interfaces;

    /// <summary>
    /// The email.
    /// </summary>
    public class Email : IEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the email owner.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string EmailOwner { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        [Required]
        [MaxLength(250)]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        [Required]
        [MaxLength(10000)]
        public string Body { get; set; }

        #endregion
    }
}