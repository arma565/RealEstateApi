using System.ComponentModel.DataAnnotations;

public class Register
{
    private string user_name = "";
    private string email = "";
    private string password = "";
    private string repeat_password = "";

    [Required]
    private bool accept_terms = false;

    [Required]
    public string UserName
    {
        get => user_name;
        set => user_name = value;
    }

    [Required(ErrorMessage = "Email is reqired!")]
    [EmailAddress(ErrorMessage = "Invalid email address!")]
    public string Email
    {
        get => email;
        set => email = value;
    }

    [Required]
    [DataType(DataType.Password)]
    public string Password
    {
        get => password;
        set => password = value;
    }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password",ErrorMessage = "The Password and Confirm Password do not match!")]
    public string RepeatPassword
    {
        get => repeat_password;
        set => repeat_password = value;
    }
    [Required]
    [Range(typeof(bool) , "true" , "true" , ErrorMessage ="You must accept the terms and conditions!")]
    public bool AcceptTerms
    {
        get => accept_terms;
        set => accept_terms = value;
    }
}
