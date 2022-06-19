using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BitakShop.Core.Models;
using BitakShop.Core.Utility;
using BitakShop.Infrastructure.Repositories;
using BitakShop.Web.ViewModels;

namespace BitakShop.Web.Areas.Customer.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private UsersRepository _userRepo;

        public AuthController()
        {
        }

        public AuthController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, UsersRepository userRepo)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            UserRepo = userRepo;
        }
        public UsersRepository UserRepo
        {
            get
            {
                return _userRepo ?? new UsersRepository();
            }
            private set
            {
                _userRepo = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Auth/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, bool registered = false)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Registered = registered;
            return View();
        }
        [AllowAnonymous]
        public ActionResult AccessDenied(string returnUrl = null)
        {
            ViewBag.ReturnUrl = "/Admin/Dashboard/Index";
            if (!string.IsNullOrEmpty(returnUrl))
                ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //
        // POST: /Auth/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(ViewModels.FullAccountingViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards Auth lockout
            // To enable password failures to trigger Auth lockout, change to shouldLockout: true
            var user = UserManager.FindByName(model.loginViewModel.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "نام کاربری یا کلمه عبور صحیح نیست.");
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.loginViewModel.Password, model.loginViewModel.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        if (!string.IsNullOrEmpty(returnUrl))
                            return RedirectToLocal(returnUrl); // Or use returnUrl

                        return RedirectToLocal("/Customer/Dashboard"); // Or use returnUrl
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.loginViewModel.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "نام کاربری یا کلمه عبور صحیح نیست.");
                    return View(model);
            }
        }

        //
        // GET: /Auth/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Auth/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FullAccountingViewModel model, string returnUrl) // async Task<ActionResult>
        {
            if (ModelState.IsValid)
            {
                #region Check for duplicate username or email
                if (UserRepo.UserNameExists(model.registerCustomerViewModel.UserName))
                {
                    ModelState.AddModelError("", "نام کاربری قبلا ثبت شده");
                    return View(model);
                }
                if (UserRepo.EmailExists(model.registerCustomerViewModel.Email))
                {
                    ModelState.AddModelError("", "ایمیل قبلا ثبت شده");
                    return View(model);
                }
                #endregion

                var user = new User { UserName = model.registerCustomerViewModel.UserName, Email = model.registerCustomerViewModel.Email, FirstName = model.registerCustomerViewModel.FirstName, LastName = model.registerCustomerViewModel.LastName };
                UserRepo.CreateUser(user, model.registerCustomerViewModel.Password);
                if (user.Id != null)
                {
                    // Add Customer Role
                    UserRepo.AddUserRole(user.Id, StaticVariables.CustomerRoleId);

                    // Add Customer
                    var customer = new Core.Models.Customer()
                    {
                        UserId = user.Id,
                        IsDeleted = false
                    };
                    UserRepo.AddCustomer(customer);
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable Auth confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your Auth", "Please confirm your Auth by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    if(!string.IsNullOrEmpty(returnUrl))
                        return Redirect("/Customer/Auth/Login/?registered=true&returnUrl="+returnUrl);
                    else
                        return Redirect("/Customer/Auth/Login/?registered=true");

                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        //
        // GET: /Auth/ForgotPasswordConfirmation
        [HttpPost]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        // GET: /Account/ExternalLoginFailure
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard");
        }
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}