using System.ComponentModel.DataAnnotations;

public class Profile
{
    private string user_name = "";
    private string first_name = "";
    private string last_name = "";
    private string phone_number = "";

    [Required]
    public string UserName { get => user_name; set => user_name = value; }
    [Required]
    public string FirstName { get => first_name; set => first_name = value; }
    [Required]
    public string LastName { get => last_name; set => last_name = value; }
    [Required]
    public string PhoneNumber { get => phone_number; set => phone_number = value; }
}