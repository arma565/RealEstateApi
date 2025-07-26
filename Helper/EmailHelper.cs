namespace RealEstate.Helper
{
    #pragma warning disable CA1515
    public class EmailHelper
    {
        // Email validation helper
       public bool IsValidEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email && email.Length <= 254; // RFC 5321 limit
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
