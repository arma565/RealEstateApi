using System.ComponentModel.DataAnnotations;

public class Recovery
{
    private string email = "";

    [Required(ErrorMessage = "Email is reqired!")]
    [EmailAddress(ErrorMessage = "Invalid email address!")]
    public string Email
    {
        get => email;
        set => email = value;
    }
}
