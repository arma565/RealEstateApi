using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Authentication
{
    public sealed class UserProfile
    {
        private string _user_name = "";
        private string _first_name = "";
        private string _last_name = "";
        private string _phone_number = "";

        [Required]
        public string UserName
        {
            get => _user_name;
            set => _user_name = value;
        }
        [Required]
        public string FirstName
        {
            get => _first_name;
            set => _first_name = value;
        }
        [Required]
        public string LastName
        {
            get => _last_name;
            set => _last_name = value;
        }
        [Required]
        public string PhoneNumber
        {
            get => _phone_number;
            set => _phone_number = value;
        }
    }
}
