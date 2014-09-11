namespace MyClientForMSExchange.Helpers
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using Core.EntityFrameworkDAL.Entities;
    using Core.EntityFrameworkDAL.Repositories;
    using Core.EntityFrameworkDAL.Repositories.Interfaces;

    using Microsoft.Ajax.Utilities;

    using MyClientForMSExchange.Models;

    using Ninject;

    /// <summary>
    /// Helper for authentication
    /// </summary>
    public class FormsAuthenticationHelper : IAuthenticationHelper
    {
        /// <summary>
        /// Gets or sets the repository clients.
        /// </summary>
        /// <value>
        /// The repository clients.
        /// </value>
        [Inject]
        public IRepository<Client> RepositoryClients { get; set; }

        /// <summary>
        /// Gets or sets the repository clients static.
        /// </summary>
        /// <value>
        /// The repository clients static.
        /// </value>
        [Inject]
        public static IRepository<Client> RepositoryClientsStatic { get; set; }

        /// <summary>
        /// Logins the specified user email.
        /// </summary>
        /// <param name="userEmail">The user email.</param>
        /// <param name="password">The password.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        /// <returns></returns>
        public bool Login(string userEmail, string password, bool isPersistent)
        {
            // var rep = new Repository<Client>(new MyClientForMSExchangeContainer());
            Client client = this.RepositoryClients.SearchFor(p => p.Email == userEmail).SingleOrDefault();

            if ((client != null) 
                && 
                (MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]) == password))
            {
                this.CreateCookie(client, isPersistent);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Log out.
        /// </summary>
        public void LogOut()
        {
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Determines whether this instance is authentificated.
        /// </summary>
        /// <returns></returns>
        public bool IsAuthentificated()
        {
            return (HttpContext.Current != null) && (HttpContext.Current.Request.Cookies.Get(FormsAuthentication.FormsCookieName) != null);
        }

        /// <summary>
        /// Creates the cookie.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        public void CreateCookie(Client client, bool isPersistent)
        {
            var ticket = new FormsAuthenticationTicket(
                  1,
                  client.Email,
                  DateTime.Now,
                  DateTime.Now.Add(FormsAuthentication.Timeout),
                  isPersistent,
                  string.Empty,
                  FormsAuthentication.FormsCookiePath);

            var encTicket = FormsAuthentication.Encrypt(ticket);
            FormsAuthentication.SetAuthCookie(encTicket, isPersistent);
            HttpContext.Current.Session["authsession"] = client;
        }

        /// <summary>
        /// Gets the current client.
        /// </summary>
        /// <value>
        /// The current client.
        /// </value>
        public static Client CurrentClient
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.User != null)
                {
                    var login = HttpContext.Current.User.Identity.Name;

                    if (login != string.Empty)
                    {
                        var formsAuthenticationTicket = FormsAuthentication.Decrypt(login);

                        if (formsAuthenticationTicket != null)
                        {
                            var name = formsAuthenticationTicket.Name;

                            var currentClient = HttpContext.Current.Session["authsession"] as Client;

                            if (currentClient == null || !currentClient.Email.Equals(name))
                            {
                                if (string.IsNullOrEmpty(login))
                                {
                                    HttpContext.Current.Session["authsession"] = null;
                                }
                                else
                                {
                                    // var rep = new Repository<Client>(new MyClientForMSExchangeContainer());
                                    var client = RepositoryClientsStatic.SearchFor(p => p.Email == formsAuthenticationTicket.Name).SingleOrDefault();

                                    HttpContext.Current.Session["authsession"] = client;
                                }
                            }
                        }

                        return HttpContext.Current.Session["authsession"] as Client;
                    }
                }

                return null;
            }
        }
    }
}