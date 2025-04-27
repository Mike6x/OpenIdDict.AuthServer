using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OpenIddict.Abstractions;

namespace Identity.Application.Authorization.ViewModels;

public class VerifyViewModel
{

    [Display(Name = "Application")]
    public string ApplicationName { get; set; } =string.Empty;

    [BindNever, Display(Name = "Error")]
    public string Error { get; set; } = string.Empty;

    [BindNever, Display(Name = "Error description")]
    public string ErrorDescription { get; set; } = string.Empty;

    [Display(Name = "Scope")]
    public string Scope { get; set; }  =string.Empty;  

    [FromQuery(Name = OpenIddictConstants.Parameters.UserCode)]
    [Display(Name = "User code")]
    public string UserCode { get; set; } = string.Empty;

}
