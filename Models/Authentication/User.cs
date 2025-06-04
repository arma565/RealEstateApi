namespace RealEstate.Models.Authentication
{
    public sealed class User
    {
        private string id = "";
        private Uri? profile_image_path;
        private string first_name = "";
        private string last_name = "";
        private bool? accept_terms = false;
        private string user_name = "";
        private string email = "";
        private string phone_number = "";

        public string Id
        {
            get => id;
            set => id = value;
        }
        public Uri? ProfileImagePath
        {
            get => profile_image_path;
            set => profile_image_path = value;
        }
        public string FirstName
        {
            get => first_name;
            set => first_name = value;
        }
        public string LastName
        {
            get => last_name;
            set => last_name = value;
        }
        public bool? AcceptTerms
        {
            get => accept_terms;
            set => accept_terms = value;
        }
        public string UserName
        {
            get => user_name;
            set => user_name = value;
        }
        public string Email
        {
            get => email;
            set => email = value;
        }
        public string PhoneNumber
        {
            get => phone_number;
            set => phone_number = value;
        }
    }
}

