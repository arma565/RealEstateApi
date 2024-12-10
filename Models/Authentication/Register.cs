using System.ComponentModel.DataAnnotations;

public class Register
{
    [Required]
    private string user_name = "";
    [Required]
    private string email = "";
    [Required]
    private string password = "";
    [Required]
    private string repeat_password = "";
    [Required]
    private bool accept_terms = false;

    public string UserName
    {
        get => user_name;
        set => user_name = value;
    }
    public string Email
    {
        get => email;
        set => email = value;
    }
    public string Password
    {
        get => password;
        set => password = value;
    }

    public string RepeatPassword
    {
        get => repeat_password;
        set => repeat_password = value;
    }

    public bool AcceptTerms
    {
        get => accept_terms;
        set => accept_terms = value;
    }
}
