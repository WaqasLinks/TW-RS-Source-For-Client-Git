using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeaveON.Models
{
  // Models returned by AccountController actions.
  public class ExternalLoginConfirmationViewModel
  {
    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "Hometown")]
    public string Hometown { get; set; }
  }

  public class ExternalLoginListViewModel
  {
    public string ReturnUrl { get; set; }
  }

  public class SendCodeViewModel
  {
    public string SelectedProvider { get; set; }
    public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    public string ReturnUrl { get; set; }
    public bool RememberMe { get; set; }
  }

  public class VerifyCodeViewModel
  {
    [Required]
    public string Provider { get; set; }

    [Required]
    [Display(Name = "Code")]
    public string Code { get; set; }
    public string ReturnUrl { get; set; }

    [Display(Name = "Remember this browser?")]
    public bool RememberBrowser { get; set; }

    public bool RememberMe { get; set; }
  }

  public class ForgotViewModel
  {
    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; }
  }

  public class LoginViewModel
  {
    [Required]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
  }

  public class RegisterViewModel
  {
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "UserName")]
    public string UserName { get; set; }
    
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Display(Name = "Hometown")]
    public string Hometown { get; set; }


    [Display(Name = "Department")]
    public int DepartmentId { get; set; }

    [Display(Name = "CountryId")]
    public int CountryId { get; set; }

    [Display(Name = "Role")]
    public string Role { get; set; }

    [Display(Name = "ITMS Id")]
    public int BioStarEmpNum { get; set; }

    [Display(Name = "Leave Policy")]
    public int UserLeavePolicyId { get; set; }

  }
  public class UpdateUserViewModel
  {
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    
    [Display(Name = "Hometown")]
    public string Hometown { get; set; }


    [Display(Name = "Department")]
    public int DepartmentId { get; set; }

    [Display(Name = "Role")]
    public string Role { get; set; }

    [Display(Name = "ITMS Id")]
    public int BioStarEmpNum { get; set; }

    [Display(Name = "Leave Policy")]
    public int UserLeavePolicyId { get; set; }

  }

 
  public class ResetPasswordViewModel
  {
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    public string Code { get; set; }
  }

  public class ForgotPasswordViewModel
  {
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
  }
}
