namespace MyClientForMSExchange.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Core.EntityFrameworkDAL.Constants;
    using Core.EntityFrameworkDAL.Entities;
    using Core.EntityFrameworkDAL.Enums;
    using Core.EntityFrameworkDAL.Repositories.Interfaces;
    using Microsoft.Exchange.WebServices.Data;
    using MyClientForMSExchange.Models;
    using Newtonsoft.Json;
    using Ninject;

    /// <summary>
    /// The ms exchange helper.
    /// </summary>
    public class MSExchangeHelper : IMSExchangeHelper
    {
        /// <summary>
        /// The rempository catalog
        /// </summary>
        [Inject]
        public IRepository<Catalog> RepositoryCatalogs { get; set; }

        /// <summary>
        /// Gets or sets the repository email items.
        /// </summary>
        [Inject]
        public IRepository<EmailItem> RepositoryEmailItems { get; set; }

        /// <summary>
        /// Gets or sets the repository clients.
        /// </summary>
        /// <value>
        /// The repository clients.
        /// </value>
        [Inject]
        public IRepository<Client> RepositoryClients { get; set; }

        /// <summary>
        /// Gets the mails inbox.
        /// </summary>
        /// <param name="enumCatalog">The enum catalog.</param>
        /// <returns></returns>
        public List<EmailSubject> GetMailsInbox(EmailCatalog enumCatalog)
        {
            var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;
            var subList = new List<EmailSubject>();
            if (client != null)
            {
                service.Credentials = new WebCredentials(
                    client.Email.Split('@')[0],
                    MyCryptoHelper.DecryptStringAES(
                        client.Password,
                        ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                foreach (var item in service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue)))
                {
                    var email = (EmailMessage)item;
                    subList.Add(new EmailSubject { Subject = email.Subject, Date = email.DateTimeCreated.ToString(CultureInfo.InvariantCulture) });
                }
            }

            return subList;
        }

        /// <summary>
        /// Deletes the email.
        /// </summary>
        /// <param name="emailSubject">The email subject.</param>
        /// <param name="dateCreation">The date creation.</param>
        /// <returns></returns>
        public List<EmailSubject> DeleteEmail(string emailSubject, string dateCreation)
        {
            var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;
            var subList = new List<EmailSubject>();
            if (client != null)
            {
                service.Credentials = new WebCredentials(
                    client.Email.Split('@')[0],
                    MyCryptoHelper.DecryptStringAES(
                        client.Password,
                        ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                foreach (var item in service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue)))
                {
                    var email = (EmailMessage)item;
                    if ((email.Subject == emailSubject) && (email.DateTimeCreated.ToString(CultureInfo.InvariantCulture) == dateCreation))
                    {
                        email.Delete(DeleteMode.MoveToDeletedItems);
                    }
                }
            }

            return subList;
        }

        /// <summary>
        /// Gets the emails.
        /// </summary>
        /// <param name="emailCatalog">
        /// The email Catalog.
        /// </param>
        /// <returns>
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
        public string GetEmails(EmailCatalog emailCatalog)
        {
            var sb = new StringBuilder();
            var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;

            if (client != null)
            {
                service.Credentials = new WebCredentials(
                    client.Email.Split('@')[0],
                    MyCryptoHelper.DecryptStringAES(
                        client.Password,
                        ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                try
                {
                    FindItemsResults<Item> emails;
                    switch (emailCatalog)
                    {
                        case EmailCatalog.Inbox:

                            emails = service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue));
                            break;
                        case EmailCatalog.SentItems:
                            emails = service.FindItems(WellKnownFolderName.SentItems, new ItemView(int.MaxValue));
                            break;
                        case EmailCatalog.DeletedItems:
                            emails = service.FindItems(WellKnownFolderName.DeletedItems, new ItemView(int.MaxValue));
                            break;
                        case EmailCatalog.Drafts:
                            emails = service.FindItems(WellKnownFolderName.Drafts, new ItemView(int.MaxValue));
                            break;
                        default:
                            emails = service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue));
                            break;
                    }

                    var catalogId =
                        this.RepositoryCatalogs.SearchFor(x => x.CatalogName == emailCatalog.ToString())
                            .Select(x => x.CatalogId)
                            .SingleOrDefault();

                    var items = service.BindToItems(
                        emails.Select(item => item.Id),
                        new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.Subject, ItemSchema.Body));
                    var emailItemCollection =
                        items.Select(
                            item =>
                            new EmailItem
                                {
                                    Body = item.Item.Body.ToString(),
                                    CreationDate = item.Item.DateTimeCreated,
                                    InternetMessageId = ((EmailMessage)item.Item).InternetMessageId,
                                    Subject = item.Item.Subject,
                                    CatalogId = catalogId
                                }).ToList();

                    foreach (var emailItem in emailItemCollection)
                    {
                        // add to db if not exist
                        EmailItem item = emailItem;
                        if (
                            !this.RepositoryEmailItems.SearchFor(
                                x => x.InternetMessageId == item.InternetMessageId && x.CatalogId == catalogId).Any())
                        {
                            this.RepositoryEmailItems.Add(emailItem);
                            this.RepositoryEmailItems.Save();
                        }
                    }

                    var allEmailsInCatalog =
                        this.RepositoryEmailItems.GetAll().Where(element => element.CatalogId == catalogId).OrderByDescending(element => element.CreationDate);

                    foreach (var item in allEmailsInCatalog)
                    {
                        var subjectWithId = new SubjectWithId
                                                {
                                                    Subject = item.Subject,
                                                    Id = item.InternetMessageId,
                                                    DateCreation =
                                                        item.CreationDate.ToString(
                                                            CultureInfo.InvariantCulture)
                                                };
                        var jsonUser = JsonConvert.SerializeObject(subjectWithId);
                        sb.Append(jsonUser + ",");
                    }

                    sb.Remove(sb.Length - 1, 1);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            return String.Format(Constants.FormatJson, sb);
        }

        /// <summary>
        /// Gets the mails sent.
        /// </summary>
        /// <param name="enumCatalog">The enum catalog.</param>
        /// <returns></returns>
        public List<EmailSubject> GetMailsSent(EmailCatalog enumCatalog)
        {
            var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;
            var subList = new List<EmailSubject>();
            if (client != null)
            {
                service.Credentials = new WebCredentials(
                    client.Email.Split('@')[0],
                    MyCryptoHelper.DecryptStringAES(
                        client.Password,
                        ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                foreach (var item in service.FindItems(WellKnownFolderName.SentItems, new ItemView(int.MaxValue)))
                {
                    var email = (EmailMessage)item;
                    subList.Add(new EmailSubject { Subject = email.Subject, Date = email.DateTimeCreated.ToString(CultureInfo.InvariantCulture) });
                }
            }

            return subList;
        }

        /// <summary>
        /// Gets the mails deleted.
        /// </summary>
        /// <param name="enumCatalog">The enum catalog.</param>
        /// <returns></returns>
        public List<EmailSubject> GetMailsDeleted(EmailCatalog enumCatalog)
        {
            var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;
            var subList = new List<EmailSubject>();
            if (client != null)
            {
                service.Credentials = new WebCredentials(
                    client.Email.Split('@')[0],
                    MyCryptoHelper.DecryptStringAES(
                        client.Password,
                        ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                foreach (var item in service.FindItems(WellKnownFolderName.DeletedItems, new ItemView(int.MaxValue)))
                {
                    var email = (EmailMessage)item;
                    subList.Add(new EmailSubject { Subject = email.Subject, Date = email.DateTimeCreated.ToString(CultureInfo.InvariantCulture) });
                }
            }

            return subList;
        }

        /// <summary>
        /// Gets the mails drafts.
        /// </summary>
        /// <param name="enumCatalog">The enum catalog.</param>
        /// <returns></returns>
        public List<string> GetMailsDrafts(EmailCatalog enumCatalog)
        {
            var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;
            var subList = new List<string>();
            if (client != null)
            {
                service.Credentials = 
                    new WebCredentials(
                        client.Email.Split('@')[0],
                        MyCryptoHelper.DecryptStringAES(
                            client.Password,
                            ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                foreach (var item in service.FindItems(WellKnownFolderName.Drafts, new ItemView(int.MaxValue)))
                {
                    var email = (EmailMessage)item;
                    subList.Add(email.Subject);
                }
        }

            return subList;
        }

        /// <summary>
        /// Gets the body email by identifier.
        /// </summary>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <param name="emailStringCatalog">
        /// The email String Catalog.
        /// </param>
        /// <returns>
        /// </returns>
        public string GetBodyEmailById(string id, string emailStringCatalog)
        {
            EmailCatalog emailCatalog;
            switch (emailStringCatalog)
            {
                case "Inbox":
                    emailCatalog = EmailCatalog.Inbox;
                    break;
                case "SentItems":
                    emailCatalog = EmailCatalog.SentItems;
                    break;
                case "DeletedItems":
                    emailCatalog = EmailCatalog.DeletedItems;
                    break;
                case "Drafts":
                    emailCatalog = EmailCatalog.Drafts;
                    break;
                default:
                    emailCatalog = EmailCatalog.Inbox;
                    break;
            }

            var catalogId = this.RepositoryCatalogs.SearchFor(x => x.CatalogName == emailCatalog.ToString())
                .Select(x => x.CatalogId)
                .SingleOrDefault();

            var body =
                this.RepositoryEmailItems.SearchFor(item => item.InternetMessageId == id && item.CatalogId == catalogId)
                    .Select(x => x.Body)
                    .SingleOrDefault();

            return body;
        }

        /// <summary>
        /// News the email.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns></returns>
        public bool NewEmail(Email emailModel)
        {
            var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;

            if (client != null)
            {
                service.Credentials = new WebCredentials(
                    client.Email.Split('@')[0],
                    MyCryptoHelper.DecryptStringAES(
                        client.Password,
                        ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                try
                {
                    // Create and save a folder associated message in the Inbox. 
                    var message = new EmailMessage(service)
                                      {
                                          Subject = emailModel.Subject, 
                                          Body = emailModel.Body
                                      };

                    message.ToRecipients.Add(emailModel.EmailOwner);
                    message.SendAndSaveCopy();
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Logins the specified login model.
        /// </summary>
        /// <param name="loginModel">The login model.</param>
        /// <returns></returns>
        public bool Login(LoginModel loginModel)
        {
            try
            {
                var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                var userName = loginModel.Email.Split('@');
                service.Credentials = new WebCredentials(userName[0], loginModel.Password);
                service.AutodiscoverUrl(loginModel.Email);
                var client = this.RepositoryClients.SearchFor(x => x.Email == loginModel.Email).SingleOrDefault();
                if (client == null)
                {
                    this.RepositoryClients.Add(
                        new Client
                            {
                                Email = loginModel.Email,
                                Password =
                                    MyCryptoHelper.EncryptStringAES(
                                        loginModel.Password,
                                        ConfigurationManager.AppSettings["KeyForAESCrypto"])
                            });

                    this.RepositoryClients.Save();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deletes the email by identifier.
        /// </summary>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <param name="emailStringCatalog">
        /// The email String Catalog.
        /// </param>
        /// <returns>
        /// </returns>
        public bool DeleteEmailById(string id, string emailStringCatalog)
        {
            EmailCatalog emailCatalog;
            switch (emailStringCatalog)
            {
                case "Inbox":
                    emailCatalog = EmailCatalog.Inbox;
                    break;
                case "SentItems":
                    emailCatalog = EmailCatalog.SentItems;
                    break;
                case "DeletedItems":
                    emailCatalog = EmailCatalog.DeletedItems;
                    break;
                case "Drafts":
                    emailCatalog = EmailCatalog.Drafts;
                    break;
                default:
                    emailCatalog = EmailCatalog.Inbox;
                    break;
            }

            var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;

            if (client != null)
            {
                service.Credentials = new WebCredentials(
                    client.Email.Split('@')[0],
                    MyCryptoHelper.DecryptStringAES(
                        client.Password,
                        ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                try
                {
                    FindItemsResults<Item> findResults;
                    switch (emailCatalog)
                    {
                        case EmailCatalog.Inbox:
                            findResults = service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue));
                            break;
                        case EmailCatalog.SentItems:
                            findResults = service.FindItems(WellKnownFolderName.SentItems, new ItemView(int.MaxValue));
                            break;
                        case EmailCatalog.DeletedItems:
                            findResults = service.FindItems(WellKnownFolderName.DeletedItems, new ItemView(int.MaxValue));
                            break;
                        case EmailCatalog.Drafts:
                            findResults = service.FindItems(WellKnownFolderName.Drafts, new ItemView(int.MaxValue));
                            break;
                        default:
                            findResults = service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue));
                            break;
                    }

                    foreach (var item in findResults)
                    {
                        var email = (EmailMessage)item;
                        if (email.InternetMessageId == id)
                        {
                            if (emailCatalog == EmailCatalog.DeletedItems)
                            {
                                email.Delete(DeleteMode.HardDelete);
                            }
                            else
                            {
                                email.Delete(DeleteMode.MoveToDeletedItems);
                            }

                    var catalogId = this.RepositoryCatalogs.SearchFor(x => x.CatalogName == emailCatalog.ToString())
                        .Select(x => x.CatalogId)
                        .SingleOrDefault();

                            var emailForDeleting = this.RepositoryEmailItems.SearchFor(
                                emailDeleting => emailDeleting.InternetMessageId == id 
                                    && emailDeleting.CatalogId == catalogId).SingleOrDefault();

                            this.RepositoryEmailItems.Delete(emailForDeleting);
                            this.RepositoryEmailItems.Save();
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }
    }
}