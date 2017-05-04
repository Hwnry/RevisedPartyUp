using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ExceptionServices;
using Newtonsoft.Json;

namespace PartyUp.Models
{
    // Models used as parameters to AccountController actions.
    /**
     * Several models are included in this .cs file and will be summarized here.
     * 
     * AddExternalLoginBindingModel provides the token authentification for 
     * logins classified as external.
     * 
     * ChangePasswordBindingModel
     * This is for future security implementation. The premise of this is that
     * the user can change their password by providing their old password
     * and twice confirmed password
     * 
     * The registration model requires that users provide data such as
     * first name, last name, twice confirmed password, and email.
     * Acceptance constraints are applied to ensure only the correct data
     * is put into the database.
     * 
     * RegisterExternalBindingModel requires the email used to login.
     * Another feature that will be implemented more in the future.
     * 
     * RemoveLoginBindingModel
     * This removes the ability of login. This looks at who provided the key
     * and what key. Expired authorization can't be used
     * 
     * SetPasswordBindingModel
     * This binding model is utilized to ensure that the new password provided
     * is correct and has been twice verified.
     * */

    public class AddExternalLoginBindingModel
    {
        //required string representing the token for external login
        [Required]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        //required string representing the old password
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        //required string of the password the user wishes to switch to 
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        //required string of the confirmed password to avoid password error
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {
        //required string of the email
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //required string of the firstname
        [Required]
        [Display(Name = "First Name")]
        public string firstName { get; set; }

        //requried string of the lastname
        [Required]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        //required string of the password with validation
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        //required string of the confirmed password to ensure validation
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterExternalBindingModel
    {
        //required string repesenting the email to login
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {

        //required string representing the login provider
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        //requried string representing the provider's key
        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        //required string representing the new password with validation
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        //required string representing the confirmed password for validation
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
