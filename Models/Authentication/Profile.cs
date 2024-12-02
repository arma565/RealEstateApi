public class Profile
{
    private string profile_image_path = "";
    private string user_name = "";
    private string first_name = "";
    private string last_name = "";
    private string phone_number = "";

    public string ProfileImagePath { get => profile_image_path; set => profile_image_path = value; }
    public string UserName { get => user_name; set => user_name = value; }
    public string FirstName { get => first_name; set => first_name = value; }
    public string LastName { get => last_name; set => last_name = value; }
    public string PhoneNumber { get => phone_number; set => phone_number = value; }
}