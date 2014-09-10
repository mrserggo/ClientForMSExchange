namespace MyClientForMSExchange.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The login model.
    /// </summary>
    public class LoginModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required]
        [StringLength(40, ErrorMessage = "The Email must be maximum 40 characters long")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the is persistent.
        /// </summary>
        [Display(Name = "Remember me?")]
        public bool IsPersistent { get; set; }

        /// <summary>
        /// Gets or sets the return url.
        /// </summary>
        public string ReturnUrl { get; set; }

        #endregion
    }
}
