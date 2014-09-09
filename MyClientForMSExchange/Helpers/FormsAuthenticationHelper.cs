using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using MyClientForMSExchange.Models.Entities;
using MyClientForMSExchange.Repositories;
using MyClientForMSExchange.Repositories.Interfaces;

namespace MyClientForMSExchange.Helpers
{
    using System.Configuration;

    using MyClientForMSExchange.Models;

    /// <summary>
    /// Helper for authentication
    /// </summary>
    public class FormsAuthenticationHelper : IAuthenticationHelper
    {
        /// <summary>
        /// Logins the specified user email.
        /// </summary>
        /// <param name="userEmail">The user email.</param>
        /// <param name="password">The password.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        /// <returns></returns>
        public Boolean Login(String userEmail, String password, Boolean isPersistent)
        {
            var rep = new Repository<Client>(new MyClientForMSEvchangeContainer());
            Client client = rep.SearchFor(p => p.Email == userEmail).SingleOrDefault();

            if ((client != null) && (MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]) == password))
            {
                //client.Password = password;
                CreateCookie(client, isPersistent);
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
        public Boolean IsAuthentificated()
        {
            return (HttpContext.Current != null) && (HttpContext.Current.Request.Cookies.Get(FormsAuthentication.FormsCookieName) != null);
        }

        /// <summary>
        /// Creates the cookie.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        public void CreateCookie(Client client, Boolean isPersistent)
        {
            var ticket = new FormsAuthenticationTicket(
                  1,
                  client.Email,
                  DateTime.Now,
                  DateTime.Now.Add(FormsAuthentication.Timeout),
                  isPersistent,
                  String.Empty,
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

                    if (login != String.Empty)
                    {
                        var formsAuthenticationTicket = FormsAuthentication.Decrypt(login);

                        if (formsAuthenticationTicket != null)
                        {
                            var name = formsAuthenticationTicket.Name;

                            var currentClient = HttpContext.Current.Session["authsession"] as Client;

                            if (currentClient == null || !currentClient.Email.Equals(name))
                            {
                                if (String.IsNullOrEmpty(login))
                                {
                                    HttpContext.Current.Session["authsession"] = null;
                                }
                                else
                                {
                                    var rep = new Repository<Client>(new MyClientForMSEvchangeContainer());
                                    var client = rep.SearchFor(p => p.Email == formsAuthenticationTicket.Name).SingleOrDefault();

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