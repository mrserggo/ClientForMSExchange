namespace Core.EntityFrameworkDAL.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The email item.
    /// </summary>
    public class EmailItem
    {
        /// <summary>
        /// Gets or sets the email item id.
        /// </summary>
        public long EmailItemId { get; set; }

        /// <summary>
        /// Gets or sets the internal message id.
        /// </summary>
        [Required]
        public string InternetMessageId { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        [Required]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        [Required]
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the catalog id.
        /// </summary>
        public long CatalogId { get; set; }

        /// <summary>
        /// Gets or sets the catalog.
        /// </summary>
        public virtual Catalog Catalog { get; set; }
    }
}