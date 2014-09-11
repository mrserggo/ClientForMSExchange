namespace Core.EntityFrameworkDAL.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The catalog.
    /// </summary>
    public class Catalog
    {
        /// <summary>
        /// Gets or sets the catalog id.
        /// </summary>
        public long CatalogId { get; set; }

        /// <summary>
        /// Gets or sets the catalog name.
        /// </summary>
        [Required]
        public string CatalogName { get; set; }

        /// <summary>
        /// Gets or sets the email items.
        /// </summary>
        public virtual ICollection<EmailItem> EmailItems { get; set; }
    }
}
