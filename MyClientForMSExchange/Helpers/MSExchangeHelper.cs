namespace MyClientForMSExchange.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Core.EntityFrameworkDAL.Entities;
    using Core.EntityFrameworkDAL.Enums;
    using Core.EntityFrameworkDAL.Repositories;
    using Core.EntityFrameworkDAL.Repositories.Interfaces;
    using Microsoft.Exchange.WebServices.Data;

    using MyClientForMSExchange.Models;

    using Newtonsoft.Json;

    /// <summary>
    /// The ms exchange helper.
    /// </summary>
    public class MSExchangeHelper : IMSExchangeHelper
    {
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
                service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
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
                service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                foreach (var item in service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue)))
                {
                    var email = (EmailMessage)item;
                    // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                    if ((email.Subject == emailSubject) && (email.DateTimeCreated.ToString() == dateCreation))
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
            StringBuilder sb = new StringBuilder();
            string format = @"[ {0} ]";
            var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;

            if (client != null)
            {
                service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                FindItemsResults<Item> emails;

                try
                {
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

                    foreach (var item in emails)
                    {
                        var email = (EmailMessage)item;
                        var subjectWithId = new SubjectWithId { Subject = email.Subject, Id = email.InternetMessageId, DateCreation = email.DateTimeCreated.ToString(CultureInfo.InvariantCulture) };
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

            return String.Format(format, sb);
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
                service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
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
                service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
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
                service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                try
                {
                    foreach (var item in service.FindItems(WellKnownFolderName.Drafts, new ItemView(int.MaxValue)))
                    {
                        var email = (EmailMessage)item;
                        subList.Add(email.Subject);
                    }
                }
                catch (Exception)
                {
                    subList.Add("Error");
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

            var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;

            service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
            service.AutodiscoverUrl(client.Email);

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

            ServiceResponseCollection<GetItemResponse> items =
                service.BindToItems(findResults.Select(item => item.Id), new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.Subject, ItemSchema.Body));
            var temp = items.Select(
                item =>
                    {
                        return new MailItem
                                   {
                                       Id = ((EmailMessage)item.Item).InternetMessageId,
                                       Body = item.Item.Body.ToString(),
                                   };
                    }).ToArray();
            var y = temp.Where(x => x.Id == id).Select(x => x.Body).SingleOrDefault();
            return y;
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
                service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                try
                {
                    // Create and save a folder associated message in the Inbox. 
                    EmailMessage message = new EmailMessage(service);
                    message.Subject = emailModel.Subject;
                    message.Body = emailModel.Body;
                    message.ToRecipients.Add(emailModel.EmailOwner);
                    message.SendAndSaveCopy();
                }
                catch (Exception ex)
                {
                    // Write error message to the console window.
                    Console.WriteLine("Error: " + ex.Message);
                    Console.ReadLine();
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
            ExchangeService service;
            try
            {
                service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                var userName = loginModel.Email.Split('@');
                service.Credentials = new WebCredentials(userName[0], loginModel.Password);
                service.AutodiscoverUrl(loginModel.Email);

                var rep = new Repository<Client>(new MyClientForMSExchangeContainer());
                var client = rep.SearchFor(x => x.Email == loginModel.Email).SingleOrDefault();
                if (client != null)
                {
                    // user is exist in db - redirect to client for MS Exchange
                }
                else
                {
                    rep.Add(
                        new Client
                            {
                                Email = loginModel.Email,
                                Password =
                                    MyCryptoHelper.EncryptStringAES(
                                        loginModel.Password,
                                        ConfigurationManager.AppSettings["KeyForAESCrypto"])
                            });

                    // Crypto.HashPassword(loginModel.Password)
                    rep.Save();
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
            FindItemsResults<Item> findResults;

            if (client != null)
            {
                service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                try
                {
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

                    foreach (
                        var item in findResults)
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