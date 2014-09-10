namespace MyClientForMSExchange.Controllers
{
    using System.Web.Mvc;

    using Core.EntityFrameworkDAL.Entities;
    using Core.EntityFrameworkDAL.Enums;

    using MyClientForMSExchange.Helpers;

    using Ninject;

    /// <summary>
    /// The home controller.
    /// </summary>
    [Authorize]
    public partial class HomeController : Controller
    {
        /// <summary>
        /// Gets or sets the forms authentication helper.
        /// </summary>
        [Inject]
        public IAuthenticationHelper FormsAuthenticationHelper { get; set; }

        /// <summary>
        /// Gets or sets the ms exchange helper.
        /// </summary>
        [Inject]
        public IMSExchangeHelper MSExchangeHelper { get; set; }

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public virtual ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// The get emails.
        /// </summary>
        /// <param name="emailStringCatalog">
        /// The email String Catalog.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [HttpGet]
        public string GetEmails(string emailStringCatalog)
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

            return MSExchangeHelper.GetEmails(emailCatalog);
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
        [HttpGet]
        [ValidateInput(false)]
        public string GetBodyEmailById(string id, string emailStringCatalog)
        {
            var result = MSExchangeHelper.GetBodyEmailById(id, emailStringCatalog);

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
            return this.View();
        }

        /// <summary>
        /// Deleteds the items.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult DeletedItems()
        {
            return this.View();
        }

        /// <summary>
        /// Draftses this instance.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Drafts()
        {
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
                {
                    return RedirectToAction(MVC.Home.SentItems());
                }

                this.ModelState.AddModelError(string.Empty, "The e-mail is unsent");
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
        public virtual ActionResult DeleteEmail(EmailSubject emailSubject)
        {
            MSExchangeHelper.DeleteEmail(emailSubject.Subject, emailSubject.Date);

            return RedirectToAction(MVC.Home.DeletedItems());
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
        [HttpGet]
        [ValidateInput(false)]
        public virtual ActionResult DeleteEmailById(string id, string emailStringCatalog)
        {
            MSExchangeHelper.DeleteEmailById(id, emailStringCatalog);

            return RedirectToAction(MVC.Home.DeletedItems());
        }
    }
}