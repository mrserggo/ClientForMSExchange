namespace MyClientForMSExchange.Controllers
{
    using System;
    using System.Web.Mvc;

    using MyClientForMSExchange.Helpers;
    using MyClientForMSExchange.Models;

    using Ninject;

    public partial class AccountController : Controller
    {
        [Inject]
        public IAuthenticationHelper FormsAuthenticationHelper { get; set; }

        [Inject]
        public IMSExchangeHelper MSExchangeHelper { get; set; }

        /// <summary>
        /// Logins the specified return URL.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Login(String returnUrl)
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
                        if (!String.IsNullOrEmpty(loginModel.ReturnUrl))
                        {
                            return Redirect(loginModel.ReturnUrl);
                        }
                        return RedirectToAction(MVC.Home.Index());
                    }
                    
                    ModelState.AddModelError(String.Empty, "Login or password is uncorrect");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Uncorrect data for enrty to the system of Client MS Exchange");
                }

                return View(loginModel);
            }

            return View(loginModel);
        }
    }
}