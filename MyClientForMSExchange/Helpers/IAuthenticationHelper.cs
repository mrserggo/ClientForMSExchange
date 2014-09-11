namespace MyClientForMSExchange.Helpers
{
    using System.Data.Entity.Core.Metadata.Edm;

    using Core.EntityFrameworkDAL.Entities;

    using Microsoft.Exchange.WebServices.Autodiscover;

    /// <summary>
    /// The AuthenticationHelper interface.
    /// </summary>
    public interface IAuthenticationHelper
    {
        #region Methods

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="isPersistent">
        /// The is persistent.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Login(string userName, string password, bool isPersistent);

        /// <summary>
        /// The log out.
        /// </summary>
        void LogOut();

        /// <summary>
        /// The is authentificated.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsAuthentificated();

        /// <summary>
        /// Creates the cookie.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="isPersistent">The is persistent.</param>
        void CreateCookie(Client user, bool isPersistent);

        /// <summary>
        /// Currents the client.
        /// </summary>
        /// <returns></returns>
        Client CurrentClient { get; }

        #endregion
    }
}