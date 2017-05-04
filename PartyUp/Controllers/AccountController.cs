using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using PartyUp.Models;
using PartyUp.Providers;
using PartyUp.Results;

/**
 * This controller handles all of the requests related to the users accounts. 
 * The controller will handles token requests, maintaining user details and data,
 * and future handling of external login requests.
 * 
 * AccountController.
 * Use for establishing usermanger and accesstokenformats.
 * 
 * UserManager.
 * Responsible for handling user details and retreiving them correctly.
 * 
 * UserInfo.
 * Responsible for handling the user's profile data.
 * 
 * GetUserInfo.
 * Returns the information about the user sending the request.
 * 
 * PostUserInfo.
 * This is the controller used to update or change the userprofile details.
 * 
 * GetManageInfo.
 * This is a controler that returns data about he user such as the number of
 * logins, where the login is from, etc.
 * 
 * ChangePassword.
 * This requires the authenticated user to confirm the old password and 
 * input a twice validated password.
 * 
 * SetPassword.
 * This requries the authenticated user to validate the password twice before it
 * is accepted.
 * 
 * Add External Login.
 * This is a future implementation project where external logins from other social
 * media with an access key can login to the application. 
 * 
 * Remove Login.
 * Removes the login capability of certain users. 
 * 
 * GetExternalLogin.
 * This controller will return external login data. 
 * 
 * Register.
 * This is the controller that is called to register new users.
 * 
 * ExternalRegister.
 * Controller for registering new users from an established third party.
 * 
 * Dispose.
 * Garbage collection for this controller.
 * 
 */
namespace PartyUp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        //private ApplicationDbContext db = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            //establishes usermanger and access token
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            //get the usermanagement 
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            //set the value of the user manager
            private set
            {
                _userManager = value;
            }
        }

        //access token get/set
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            //get external login data
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            //verify the current user
            var currentUser = RequestContext.Principal.Identity.GetUserId();
            //get the information about the user
            var user = UserManager.FindById(currentUser);
            //return that users data
            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                firstName = user.firstName,
                lastName = user.lastName,
                PhoneNumber = user.PhoneNumber,
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null,
                UserId = user.Id
            };
        }

        // POST api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel PostUserInfo(UserInfoViewModel temp)
        {
            //get external login data
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            //verify the current user
            var currentUser = RequestContext.Principal.Identity.GetUserId();
            //get the users profiel details
            var user = UserManager.FindById(currentUser);
            //capture details of the user
            user.firstName = temp.firstName;
            user.lastName = temp.lastName;
            user.PhoneNumber = temp.PhoneNumber;
            //update user profile
            UserManager.Update(user);
            //return the update user profile 
            return new UserInfoViewModel
            {
                Email = user.Email,
                firstName = user.firstName,
                lastName = user.lastName,
                PhoneNumber = user.PhoneNumber,
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            //logout the users
            Authentication.SignOut(DefaultAuthenticationTypes.ExternalBearer);
            //return status ok
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            //get the user id
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            //if the user is not found
            if (user == null)
            {
                //return null
                return null;
            }
            //get the list of logins
            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();
            //for every login
            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                //add the login provider and the key used
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            //if the password hash is not
            if (user.PasswordHash != null)
            {
                //add the information about the login
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            //return this data 
            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            //if the model statide is not valid
            if (!ModelState.IsValid)
            {
                //return bad request
                return BadRequest(ModelState);
            }

            //get the identity result from the database
            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            //if the request failed
            if (!result.Succeeded)
            {
                //return error message
                return GetErrorResult(result);
            }

            //otherwise return status ok
            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            //check to see if the model state is valid
            if (!ModelState.IsValid)
            {
                //return badrequest
                return BadRequest(ModelState);
            }

            //get the identity result
            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            //return if the result was successful or not
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            //check the model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //call for signout
            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            //capture the external access token
            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            //check to see if the ticket is valid
            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }


            //get the external data associated with the ticket
            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            //check if there is any data
            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            //capture the results from the request
            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            //if it does not succeed, return an error; else return ok
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            //check the model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            //check to see if the model login provider is teh same as the localprovider
            if (model.LoginProvider == LocalLoginProvider)
            {
                //remove password
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                //else remove password and establish new user
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            //check to see it worked
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            //check if there is an error
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            //check to see if the user identity is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            //get the external login data from the user
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            //check to see if there is any data
            if (externalLogin == null)
            {
                return InternalServerError();
            }

            //check to see if the login does not match the provider
            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            //get the data about the user
            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            //check to see if the user has registered
            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                //perform basic authentification 
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                //look through various claims
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            //if it made it through, it worked 

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            //get authentification types
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            //check to see if generated state
            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            //for every description
            foreach (AuthenticationDescription description in descriptions)
            {
                //create a new login view model
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //create a new user
            var user = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email,
                firstName = model.firstName,
                lastName = model.lastName
            };

            //add this new users
            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            //check to see if this operation was successful 
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //get the info about the external login
            var info = await Authentication.GetExternalLoginInfoAsync();
            //check to see if info was found
            if (info == null)
            {
                return InternalServerError();
            }
            //create a new application user
            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            //get the results of creating this new user
            IdentityResult result = await UserManager.CreateAsync(user);
            //check to see if the operation worked
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            //att the login information for future reference
            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }


        /**
         * 
         * These are the helper functions for this controller.
         * 
         *  Authentification.
         * Verfies the user token.  
         * 
         * GetErrorResult.
         * Returns the result of the error, i.e. internal server errors
         * 
         * ExternalLoginData.
         * Class used to help maintain external login information such as claims
         * and login attempts. 
         * 
         * RandomOAuthStateGenerator.
         * Used for generating the authentification tokens for each user. 
         */
        #region Helpers

        private IAuthenticationManager Authentication
        {
            //authenticate token
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            //check to see if result is null
            if (result == null)
            {
                return InternalServerError();
            }

            //check to see if result was successful
            if (!result.Succeeded)
            {
                //check to see if result was an error
                if (result.Errors != null)
                {
                    //for each string, output it
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                //check to see if model state is valid
                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                //return badrequest
                return BadRequest(ModelState);
            }

            //return null 
            return null;
        }

        private class ExternalLoginData
        {
            //string for login provider
            public string LoginProvider { get; set; }
            //string for provider key
            public string ProviderKey { get; set; }
            //string for username
            public string UserName { get; set; }

            //list of all the claims
            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }
                //return all the claims
                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                //check if identity is null
                if (identity == null)
                {
                    return null;
                }

                //get the claim
                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                //check the claims validity
                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                //check the claim issue
                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                //return the external login data
                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            //seed random number generator
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            //generates random numbers of a certain legnth
            public static string Generate(int strengthInBits)
            {
                //set byte size
                const int bitsPerByte = 8;

                //check the size
                if (strengthInBits % bitsPerByte != 0)
                {
                    //wrong size throws exception
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                //find the strength
                int strengthInBytes = strengthInBits / bitsPerByte;

                //create a new seed of the token 
                byte[] data = new byte[strengthInBytes];
                //generate the random token
                _random.GetBytes(data);
                //return the token 
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
