namespace MyClientForMSExchange.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.Exchange.WebServices.Data;
    using MyClient.Core.Enums;
    using MyClientForMSExchange.Models;
    using MyClientForMSExchange.Models.Entities;
    using MyClientForMSExchange.Repositories.Interfaces;
    using Newtonsoft.Json;

    public class MSExchangeHelper : IMSExchangeHelper
    {

        //[Inject]
        //public IAuthenticationHelper FormsAuthenticationHelper { get; set; }

        /// <summary>
        /// Gets the mails inbox.
        /// </summary>
        /// <param name="enumCatalog">The enum catalog.</param>
        /// <returns></returns>
        public List<Enteties.EmailSubject> GetMailsInbox(EmailCatalog enumCatalog)
        {
            var service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;
            var subList = new List<Enteties.EmailSubject>();
            if (client != null)
            {
                service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                service.AutodiscoverUrl(client.Email);

                try
                {
                    FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue));

                    foreach (EmailMessage email in service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue)))
                    {
                        subList.Add(new Enteties.EmailSubject() {Subject = email.Subject, Date = email.DateTimeCreated.ToString(CultureInfo.InvariantCulture)});
                    }
                }
                catch (Exception ex)
                {
                    //
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
        public List<Enteties.EmailSubject> DeleteEmail(string emailSubject, string dateCreation)
        {
            ExchangeService _service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;
            var subList = new List<Enteties.EmailSubject>();
            if (client != null)
            {
                _service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                _service.AutodiscoverUrl(client.Email);

                try
                {
                    foreach (EmailMessage email in _service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue)))
                    {
                        if ((email.Subject == emailSubject) && (email.DateTimeCreated.ToString() == dateCreation))
                        {
                            email.Delete(DeleteMode.MoveToDeletedItems);
                        }     
                    }
                }
                catch (Exception ex)
                {
                    //
                }
            }

            return subList;
        }

        /// <summary>
        /// Gets the emails.
        /// </summary>
        /// <param name="eCatalog">The e catalog.</param>
        /// <returns></returns>
        public string GetEmails(EmailCatalog eCatalog)
        {
            StringBuilder sb = new StringBuilder();
            string format = @"[ {0} ]";
            ExchangeService _service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;

            if (client != null)
            {
                _service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                _service.AutodiscoverUrl(client.Email);

                FindItemsResults<Item> emails;

                try
                {
                    switch (eCatalog)
                    {
                        case EmailCatalog.Inbox:
                            emails = _service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue));
                            break;
                        case EmailCatalog.SentItems: 
                            emails = _service.FindItems(WellKnownFolderName.SentItems, new ItemView(int.MaxValue));
                            break;
                        case EmailCatalog.DeletedItems:
                            emails = _service.FindItems(WellKnownFolderName.DeletedItems, new ItemView(int.MaxValue));
                            break;
                        case EmailCatalog.Drafts:
                            emails = _service.FindItems(WellKnownFolderName.Drafts, new ItemView(int.MaxValue));
                            break;
                        default:
                            emails = _service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue));
                            break;
                    }
                    foreach (EmailMessage email in emails)
                    {
                        Enteties.SubjectWithId sWithId = new Enteties.SubjectWithId() { Subject = email.Subject, Id = email.InternetMessageId, DateCreation = email.DateTimeCreated.ToString() };
                        var jsonUser = JsonConvert.SerializeObject(sWithId);
                        sb.Append(jsonUser + ",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                catch (Exception ex)
                {
                    //
                }
            }

            return String.Format(format, sb.ToString());
        }

        /// <summary>
        /// Gets the mails sent.
        /// </summary>
        /// <param name="enumCatalog">The enum catalog.</param>
        /// <returns></returns>
        public List<Enteties.EmailSubject> GetMailsSent(EmailCatalog enumCatalog)
        {
            ExchangeService _service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;
            var subList = new List<Enteties.EmailSubject>();
            if (client != null)
            {
                _service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                _service.AutodiscoverUrl(client.Email);

                try
                {
                    foreach (EmailMessage email in _service.FindItems(WellKnownFolderName.SentItems, new ItemView(int.MaxValue)))
                    {
                        subList.Add(new Enteties.EmailSubject() { Subject = email.Subject, Date = email.DateTimeCreated.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    //subList.Add("Error");
                }
            }
            return subList;
        }

        /// <summary>
        /// Gets the mails deleted.
        /// </summary>
        /// <param name="enumCatalog">The enum catalog.</param>
        /// <returns></returns>
        public List<Enteties.EmailSubject> GetMailsDeleted(EmailCatalog enumCatalog)
        {
            ExchangeService _service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;
            var subList = new List<Enteties.EmailSubject>();
            if (client != null)
            {
                _service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                _service.AutodiscoverUrl(client.Email);

                try
                {
                    foreach (EmailMessage email in _service.FindItems(WellKnownFolderName.DeletedItems, new ItemView(int.MaxValue)))
                    {
                        subList.Add(new Enteties.EmailSubject() { Subject = email.Subject, Date = email.DateTimeCreated.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    //subList.Add("Error");
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
            ExchangeService _service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;
            var subList = new List<string>();
            if (client != null)
            {
                _service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                _service.AutodiscoverUrl(client.Email);

                try
                {
                    FindItemsResults<Item> findResults = _service.FindItems(WellKnownFolderName.Drafts, new ItemView(int.MaxValue));

                    foreach (EmailMessage email in _service.FindItems(WellKnownFolderName.Drafts, new ItemView(int.MaxValue)))
                    {
                        subList.Add(email.Subject);
                    }
                }
                catch (Exception ex)
                {
                    subList.Add("Error");
                }
            }
            return subList;
        }

        /// <summary>
        /// Gets the body email by identifier.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <param name="eStringCatalog">The e string catalog.</param>
        /// <returns></returns>
        public string GetBodyEmailById(string Id, string eStringCatalog)
        {
            EmailCatalog eCatalog;
            switch (eStringCatalog)
            {
                case "Inbox":
                    eCatalog = EmailCatalog.Inbox;
                    break;
                case "SentItems":
                    eCatalog = EmailCatalog.SentItems;
                    break;
                case "DeletedItems":
                    eCatalog = EmailCatalog.DeletedItems;
                    break;
                case "Drafts":
                    eCatalog = EmailCatalog.Drafts;
                    break;
                default:
                    eCatalog = EmailCatalog.Inbox;
                    break;
            }

            StringBuilder sb = new StringBuilder();
            string format = @"[ {0} ]";
            ExchangeService _service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;

            _service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
            _service.AutodiscoverUrl(client.Email);

            FindItemsResults<Item> findResults;
            switch (eCatalog)
            {
                case EmailCatalog.Inbox:
                    findResults = _service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue));
                    break;
                case EmailCatalog.SentItems:
                    findResults = _service.FindItems(WellKnownFolderName.SentItems, new ItemView(int.MaxValue));
                    break;
                case EmailCatalog.DeletedItems:
                    findResults = _service.FindItems(WellKnownFolderName.DeletedItems, new ItemView(int.MaxValue));
                    break;
                case EmailCatalog.Drafts:
                    findResults = _service.FindItems(WellKnownFolderName.Drafts, new ItemView(int.MaxValue));
                    break;
                default:
                    findResults = _service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue));
                    break;
            }

            ServiceResponseCollection<GetItemResponse> items =
                _service.BindToItems(findResults.Select(item => item.Id), new PropertySet(BasePropertySet.FirstClassProperties, EmailMessageSchema.Subject, EmailMessageSchema.Body));
            var temp = items.Select(
                item =>
                {
                    return new Enteties.MailItem()
                    {
                        Id =
                            ((EmailMessage)item.Item).InternetMessageId,
                        Body = item.Item.Body.ToString(),
                    };
                }).ToArray();
            var y = temp.Where(x => x.Id == Id).Select(x => x.Body).SingleOrDefault();
            return y;
        }

        /// <summary>
        /// News the email.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns></returns>
        public bool NewEmail(Email emailModel)
        {
            ExchangeService _service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;

            if (client != null)
            {
                _service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                _service.AutodiscoverUrl(client.Email);

                try
                {
                    // Create and save a folder associated message in the Inbox. 
                    EmailMessage message = new EmailMessage(_service);
                    message.Subject = emailModel.Subject;
                    message.Body = emailModel.Body;
                    message.ToRecipients.Add(emailModel.EmailOwner);
                    message.SendAndSaveCopy();

                    // Write confirmation message to the window.

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
            ExchangeService _service;
            try
            {
                _service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                var userName = loginModel.Email.Split('@');
                _service.Credentials = new WebCredentials(userName[0], loginModel.Password);
                _service.AutodiscoverUrl(loginModel.Email);

                var rep = new Repository<Client>(new MyClientForMSExchangeContainer());
                var client = rep.SearchFor(x => x.Email == loginModel.Email).SingleOrDefault();
                if (client != null)
                {
                    // user is exist in db - redirect to client for MS Exchange
                }
                else
                {
                    rep.Insert(new Client()
                                   {
                                       Email = loginModel.Email,
                                       Password = MyCryptoHelper.EncryptStringAES(loginModel.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"])//Password = loginModel.Password
                                   });//Crypto.HashPassword(loginModel.Password)
                    rep.Save();
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Deletes the email by identifier.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <param name="eStringCatalog">The e string catalog.</param>
        /// <returns></returns>
        public bool DeleteEmailById(string Id, string eStringCatalog)
        {
            EmailCatalog eCatalog;
            switch (eStringCatalog)
            {
                case "Inbox":
                    eCatalog = EmailCatalog.Inbox;
                    break;
                case "SentItems":
                    eCatalog = EmailCatalog.SentItems;
                    break;
                case "DeletedItems":
                    eCatalog = EmailCatalog.DeletedItems;
                    break;
                case "Drafts":
                    eCatalog = EmailCatalog.Drafts;
                    break;
                default:
                    eCatalog = EmailCatalog.Inbox;
                    break;
            }

            var _service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            var client = FormsAuthenticationHelper.CurrentClient;
            FindItemsResults<Item> findResults;

            if (client != null)
            {
                _service.Credentials = new WebCredentials(client.Email.Split('@')[0], MyCryptoHelper.DecryptStringAES(client.Password, ConfigurationManager.AppSettings["KeyForAESCrypto"]));
                _service.AutodiscoverUrl(client.Email);

                try
                {
                    switch (eCatalog)
                    {
                        case EmailCatalog.Inbox:
                            findResults = _service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue));
                            break;
                        case EmailCatalog.SentItems:
                            findResults = _service.FindItems(WellKnownFolderName.SentItems, new ItemView(int.MaxValue));
                            break;
                        case EmailCatalog.DeletedItems:
                            findResults = _service.FindItems(WellKnownFolderName.DeletedItems, new ItemView(int.MaxValue));
                            break;
                        case EmailCatalog.Drafts:
                            findResults = _service.FindItems(WellKnownFolderName.Drafts, new ItemView(int.MaxValue));
                            break;
                        default:
                            findResults = _service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue));
                            break;
                    }
                    foreach (
                        EmailMessage email in findResults)
                    {
                        if (email.InternetMessageId == Id)
                        {
                            if (eCatalog == EmailCatalog.DeletedItems)
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
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }

    }
}