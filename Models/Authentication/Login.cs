using System.ComponentModel.DataAnnotations;

public class Login
{
    [Required]
    string user_name = "";
    [Required]
    string password = "";

    public string UserName { get => user_name; set => user_name = value; }
    public string Password { get => password; set => password = value; }
}