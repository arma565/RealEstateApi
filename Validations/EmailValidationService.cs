namespace RealEstate.Validations;

#pragma warning disable CA1515
public class EmailValidationService
{
    // Email validation helper
    public bool IsValidEmail(string? email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email!);
            return addr.Address == email && email?.Length <= 254; // RFC 5321 limit
        }
        catch (NullReferenceException) { 
            return false;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

