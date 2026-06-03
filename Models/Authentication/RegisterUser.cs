using System.ComponentModel.DataAnnotations;

#pragma warning disable CA1515
namespace RealEstate.Models.Authentication;

public sealed class RegisterUser
{
    private string _user_name = "";
    private string _email = "";
    private string _password = "";
    private string _repeat_password = "";
    private bool accept_terms;

    [Required(ErrorMessage = "UserName is required!")]
    public string UserName
    {
        get => _user_name;
        set => _user_name = value;
    }

    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress(ErrorMessage = "Invalid email address!")]
    public string Email
    {
        get => _email;
        set => _email = value;
    }

    [Required(ErrorMessage = "Password is required!")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "The password must be more than 8 characters!")]
    public string Password
    {
        get => _password;
        set => _password = value;
    }

    [Required(ErrorMessage = "RepeatPassword is required!")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The Password and Confirm Password do not match!")]
    [MinLength(8, ErrorMessage = "The password must be more than 8 characters!")]
    public string RepeatPassword
    {
        get => _repeat_password;
        set => _repeat_password = value;
    }

    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions!")]
    public bool AcceptTerms
    {
        get => accept_terms;
        set => accept_terms = value;
    }
}

