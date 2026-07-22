using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable CA1515
namespace RealEstate.Models.Users;

public class User : IdentityUser
{
    private string _id = "";
    private string? _userName = "";
    private string? _email = "";
    private string? _phoneNumber = "";
    private string _firstName = "";
    private string _lastName = "";
    private bool _acceptTerms;
    private UserProfileImage? _profileImage;

    [Key]
    public override string Id
    {
        get => _id;
        set => _id = value;
    }

    public override string? UserName
    {
        get => _userName;
        set => _userName = value;
    }

    public override string? Email
    {
        get => _email;
        set => _email = value;
    }

    public override string? PhoneNumber
    {
        get => _phoneNumber;
        set => _phoneNumber = value;
    }

    public string FirstName
    {
        get => _firstName;
        set => _firstName = value;
    }
    public string LastName
    {
        get => _lastName;
        set => _lastName = value;
    }

    [Required]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions!")]
    public bool AcceptTerms
    {
        get => _acceptTerms;
        set => _acceptTerms = value;
    }

    public UserProfileImage? ProfileImage
    {
        get => _profileImage;
        set => _profileImage = value;
    }

}

