using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClientForMSExchange.Helpers
{
    using Microsoft.Exchange.WebServices.Data;

    using MyClient.Core.Enums;

    using MyClientForMSExchange.Models;
    using MyClientForMSExchange.Models.Entities;

    public interface IMSExchangeHelper
    {
        List<Enteties.EmailSubject> GetMailsInbox(EmailCatalog enumCatalog);

        List<Enteties.EmailSubject> DeleteEmail(string emailSubject, string dateCreation);


        string GetEmails(EmailCatalog eCatalog);

        List<Enteties.EmailSubject> GetMailsSent(EmailCatalog enumCatalog);

        List<Enteties.EmailSubject> GetMailsDeleted(EmailCatalog enumCatalog);

        List<string> GetMailsDrafts(EmailCatalog enumCatalog);

        string GetBodyEmailById(string Id, string eStringCatalog);

        bool NewEmail(Email emailModel);

        bool Login(LoginModel loginModel);

        bool DeleteEmailById(string Id, string eStringCatalog);
    }
}
