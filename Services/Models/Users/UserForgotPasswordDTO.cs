using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Models.Users;

#pragma warning disable CA1515
public class UserForgotPasswordDTO
{
    private string _email = "";

    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email
    {
        get => _email;
        set => _email = value;
    }
}

