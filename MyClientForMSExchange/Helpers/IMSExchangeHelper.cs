namespace MyClientForMSExchange.Helpers
{
    using System.Collections.Generic;

    using Core.EntityFrameworkDAL.Entities;
    using Core.EntityFrameworkDAL.Enums;

    using MyClientForMSExchange.Models;

    /// <summary>
    /// The MSExchangeHelper interface.
    /// </summary>
    public interface IMSExchangeHelper
    {
        /// <summary>
        /// The get mails inbox.
        /// </summary>
        /// <param name="enumCatalog">
        /// The enum catalog.
        /// </param>
        /// <returns>
        /// The <see />.
        /// </returns>
        List<EmailSubject> GetMailsInbox(EmailCatalog enumCatalog);

        /// <summary>
        /// The delete email.
        /// </summary>
        /// <param name="emailSubject">
        /// The email subject.
        /// </param>
        /// <param name="dateCreation">
        /// The date creation.
        /// </param>
        /// <returns>
        /// The <see></see>
        ///     .
        /// </returns>
        List<EmailSubject> DeleteEmail(string emailSubject, string dateCreation);

        /// <summary>
        /// The get emails.
        /// </summary>
        /// <param name="emailCatalog">
        /// The email Catalog.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetEmails(EmailCatalog emailCatalog);

        /// <summary>
        /// The get mails sent.
        /// </summary>
        /// <param name="enumCatalog">
        /// The enum catalog.
        /// </param>
        /// <returns>
        /// The <see />.
        /// </returns>
        List<EmailSubject> GetMailsSent(EmailCatalog enumCatalog);

        /// <summary>
        /// The get mails deleted.
        /// </summary>
        /// <param name="enumCatalog">
        /// The enum catalog.
        /// </param>
        /// <returns>
        /// The <see />.
        /// </returns>
        List<EmailSubject> GetMailsDeleted(EmailCatalog enumCatalog);

        /// <summary>
        /// The get mails drafts.
        /// </summary>
        /// <param name="enumCatalog">
        /// The enum catalog.
        /// </param>
        /// <returns>
        /// The <see />.
        /// </returns>
        List<string> GetMailsDrafts(EmailCatalog enumCatalog);

        /// <summary>
        /// The get body email by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="emailStringCatalog">
        /// The email String Catalog.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetBodyEmailById(string id, string emailStringCatalog);

        /// <summary>
        /// The new email.
        /// </summary>
        /// <param name="emailModel">
        /// The email model.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool NewEmail(Email emailModel);

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="loginModel">
        /// The login model.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Login(LoginModel loginModel);

        /// <summary>
        /// The delete email by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="emailStringCatalog">
        /// The email String Catalog.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool DeleteEmailById(string id, string emailStringCatalog);
    }
}
