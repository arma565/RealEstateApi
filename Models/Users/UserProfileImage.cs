#pragma warning disable CA1515
namespace RealEstate.Models.Users;

public sealed class UserProfileImage
{

    private Guid _id;
    private string _profileImageName = "";
    private string userId = "";
    private User? _user;

    public Guid Id
    {
        get => _id;
        set => _id = value;
    }

    public string ProfileImageName
    {
        get => _profileImageName;
        set => _profileImageName = value;
    }

    public string UserID
    {
        get => userId;
        set => userId = value;
    }

    public User? User
    {
        get => _user;
        set => _user = value;
    }
}
