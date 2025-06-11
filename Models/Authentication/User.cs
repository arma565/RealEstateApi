using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models.Authentication
{
    public sealed class User
    {
        private string _id = "";
        private string? _profile_image_name;
        private string _first_name = "";
        private string _last_name = "";
        private bool? _accept_terms = false;
        private string _user_name = "";
        private string _email = "";
        private string _phone_number = "";

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string? ProfileImagePath
        {
            get => _profile_image_name;
            set => _profile_image_name = value;
        }
        public string FirstName
        {
            get => _first_name;
            set => _first_name = value;
        }
        public string LastName
        {
            get => _last_name;
            set => _last_name = value;
        }
        public bool? AcceptTerms
        {
            get => _accept_terms;
            set => _accept_terms = value;
        }
        public string UserName
        {
            get => _user_name;
            set => _user_name = value;
        }
        public string Email
        {
            get => _email;
            set => _email = value;
        }
        public string PhoneNumber
        {
            get => _phone_number;
            set => _phone_number = value;
        }
    }
}

