namespace MyClientForMSExchange.Controllers
{
    using System.Web.Mvc;
    using Core.EntityFrameworkDAL.Constants;
    using MyClientForMSExchange.Helpers;
    using MyClientForMSExchange.Models;
    using Ninject;

    /// <summary>
    /// The account controller.
    /// </summary>
    public partial class AccountController : Controller
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
        /// Logins the specified return URL.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Login(string returnUrl)
        {
            if (FormsAuthenticationHelper.IsAuthentificated())
            {
                return RedirectToAction(MVC.Home.Index());
            }

            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        /// <summary>
        /// Logins the specified login model.
        /// </summary>
        /// <param name="loginModel">The login model.</param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                if (MSExchangeHelper.Login(loginModel))
                {
                    var isAuthentificated = FormsAuthenticationHelper.Login(loginModel.Email, loginModel.Password, loginModel.IsPersistent);
                    if (isAuthentificated)
                    {
                        if (!string.IsNullOrEmpty(loginModel.ReturnUrl))
                        {
                            return Redirect(loginModel.ReturnUrl);
                        }

                        return RedirectToAction(MVC.Home.Index());
                    }

                    ModelState.AddModelError(string.Empty, Constants.LoginorPasswordisUncorrectErrorMessage);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Constants.UncorrectDataforEnrtytotheSystemofClientMSExchangeErrorMessage);
                }

                return View(loginModel);
            }

            return View(loginModel);
        }
    }
}