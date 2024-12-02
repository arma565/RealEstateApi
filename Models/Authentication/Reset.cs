public class Reset
{
    private string email = "";
    private string token = "";
    private string new_password = "";
    private string repeat_new_password = "";

    public string Email { get => email; set => email = value; }
    public string Token { get => token; set => token = value; }
    public string NewPassword { get => new_password; set => new_password = value; }
    public string RepeatNewPassword { get => repeat_new_password; set => repeat_new_password = value; }
}