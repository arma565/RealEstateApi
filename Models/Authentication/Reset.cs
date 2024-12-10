using System.ComponentModel.DataAnnotations;

public class Reset
{
    [Required]
    private string email = "";
    [Required]
    private string token = "";
    [Required]
    private string new_password = "";
    [Required]
    private string repeat_new_password = "";

    public string Email { get => email; set => email = value; }
    public string Token { get => token; set => token = value; }
    public string NewPassword { get => new_password; set => new_password = value; }
    public string RepeatNewPassword { get => repeat_new_password; set => repeat_new_password = value; }
}