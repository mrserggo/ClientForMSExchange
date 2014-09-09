using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyClientForMSExchange.Helpers
{
    using MyClientForMSExchange.Models.Entities;

    public interface IAuthenticationHelper
    {
        #region Methods

        Boolean Login(String userName, String password, Boolean isPersistent);

        void LogOut();

        Boolean IsAuthentificated();

        /// <summary>
        /// Creates the cookie.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="isPersistent">The is persistent.</param>
        void CreateCookie(Client user, Boolean isPersistent);

        #endregion
    }
}