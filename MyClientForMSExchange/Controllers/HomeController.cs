using System;
using System.Web.Mvc;
using MyClient.Core.Enums;
using MyClientForMSExchange.Helpers;
using MyClientForMSExchange.Models.Entities;
using Ninject;

namespace MyClientForMSExchange.Controllers
{

    [Authorize]
    public partial class HomeController : Controller
    {
        [Inject]
        public IAuthenticationHelper FormsAuthenticationHelper { get; set; }

        [Inject]
        public IMSExchangeHelper MSExchangeHelper { get; set; }

        public virtual ActionResult Index()
        {
            //var subList = MSExchangeHelper.GetMailsInbox(EmailCatalog.Inbox);
            //return View(subList);
            return View();
        }

        [HttpGet]
        public string GetEmails(string eStringCatalog)
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
            return MSExchangeHelper.GetEmails(eCatalog);
        }

        /// <summary>
        /// Gets the body email by identifier.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <param name="eStringCatalog">The e string catalog.</param>
        /// <returns></returns>
        [HttpGet]
        [ValidateInput(false)]
        public string GetBodyEmailById(string Id, string eStringCatalog)
        {
            var result = MSExchangeHelper.GetBodyEmailById(Id, eStringCatalog);
            return result;
        }

        /// <summary>
        /// Logs the out.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult LogOut()
        {
            FormsAuthenticationHelper.LogOut();
            return RedirectToAction(MVC.Home.Index());
        }

        /// <summary>
        /// Sents the items.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult SentItems()
        {
            //var subList = MSExchangeHelper.GetMailsSent(EmailCatalog.SentItems);
            //return View(subList);
            return this.View();
        }

        /// <summary>
        /// Deleteds the items.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult DeletedItems()
        {
            //var subList = MSExchangeHelper.GetMailsDeleted(EmailCatalog.DeletedItems);
            //return View(subList);
            return this.View();
        }

        /// <summary>
        /// Draftses this instance.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Drafts()
        {
            //var subList = MSExchangeHelper.GetMailsDrafts(EmailCatalog.Inbox);
            //return View(subList);
            return this.View();
        }

        /// <summary>
        /// News the email.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult NewEmail()
        {
            return View();
        }

        /// <summary>
        /// News the email.
        /// </summary>
        /// <param name="emailModel">The email model.</param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult NewEmail(Email emailModel)
        {
            if (ModelState.IsValid)
            {
                if (MSExchangeHelper.NewEmail(emailModel))
                return RedirectToAction(MVC.Home.SentItems());
                else
                {
                    ViewBag.message = "Message is unsent";
                    ModelState.AddModelError(String.Empty, "The e-mail is unsent");
                }
            }
            
            return View(emailModel);
        }

        /// <summary>
        /// Deletes the email.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult DeleteEmail()
        {
            return View();
        }

        /// <summary>
        /// Deletes the email.
        /// </summary>
        /// <param name="emailSubject">The email subject.</param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult DeleteEmail(Enteties.EmailSubject emailSubject)
        {
            MSExchangeHelper.DeleteEmail(emailSubject.Subject, emailSubject.Date);
            return RedirectToAction(MVC.Home.DeletedItems());
        }

        /// <summary>
        /// Deletes the email by identifier.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <param name="eStringCatalog">The e string catalog.</param>
        /// <returns></returns>
        [HttpGet]
        [ValidateInput(false)]
        public virtual ActionResult DeleteEmailById(string Id, string eStringCatalog)
        {
            MSExchangeHelper.DeleteEmailById(Id, eStringCatalog);
            return RedirectToAction(MVC.Home.DeletedItems());
        }
    }
}