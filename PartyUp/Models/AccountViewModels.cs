using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PartyUp.Models
{
    // Models returned by AccountController actions.
    /**
     * This contains serveral view models and features that have not yet
     * been implemented. The external login view model is for the future
     * implementation of allowing for social media plugins.
     * The ManageInfoViewModel allows for the potential to view and keep track
     * of user data such as logins and external login providers. Somewhat of a
     * future security feature.
     * The UserInfoViewModel will return all of the basic information relating
     * to a user. 
     * Lastly the login provider is another potential future implementation.
     * It will keep track of login providers and the key that was provided.
     * An extended authentification feature
     * */

    public class ExternalLoginViewModel
    {
        //string for the name of the external login
        public string Name { get; set; }
        //string for the url of the external login
        public string Url { get; set; }
        //string representing the state of the login
        public string State { get; set; }
    }

    public class ManageInfoViewModel
    {
        //string for th local login provider
        public string LocalLoginProvider { get; set; }
        //string for the email provided
        public string Email { get; set; }
        //Enumerable colleciton of logins (more info contained in UserLoginInfoViewModels)
        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }
        //enumerable colection external login providers (more info contained in the ExternalLoginViewModel)
        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel
    {
        //string with the user email
        public string Email { get; set; }
        //string with the user firstName
        public string firstName { get; set; }
        //strig with the user lastName
        public string lastName { get; set; }
        //boolean confirming registration
        public bool HasRegistered { get; set; }
        //string of the login provider
        public string LoginProvider { get; set; }
        //strin of the UserId
        public string UserId { get; set; }

        //string representing the user phonenumber
        public string PhoneNumber { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        //string with the login provider
        public string LoginProvider { get; set; }
        //string with the provider key
        public string ProviderKey { get; set; }
    }
}
