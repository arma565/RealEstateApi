using System.ComponentModel.DataAnnotations;

public class Recovery
{
    [Required]
    private string email = "";

    public string Email { get => email; set => email = value; }
}