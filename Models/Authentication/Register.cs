public class Register
{
    private string user_name = "";
    private string email = "";
    private string password = "";
    private string repeat_password = "";
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
