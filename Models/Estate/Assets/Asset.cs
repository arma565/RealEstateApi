using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;


namespace RealEstate.Models.Estate.Assets
{
    public sealed class Asset
    {
        public Asset() { }

        public Asset(
            string? name,
            string? platesNumber,
            string? area,
            string? constructionYear,
            string? price,
            string? deposit,
            string? rentAmount,
            string? payment,
            string? address,
            string? description,
            string? time,
            string? date,
            string? type,
            bool water,
            bool electricity,
            bool gas,
            bool phone,
            ICollection<AssetImage> assetImages,
            ICollection<Person> persons
        )
        {
            _name = name;
            _platesNumber = platesNumber;
            _area = area;
            _constructionYear = constructionYear;
            _price = price;
            _deposit = deposit;
            _rentAmount = rentAmount;
            _payment = payment;
            _address = address;
            _description = description;
            _time = time;
            _date = date;
            _type = type;
            _water = water;
            _electricity = electricity;
            _gas = gas;
            _phone = phone;
            _assetImages = assetImages;
            _persons = persons;
        }

        private Guid _id;
        private string? _name = "";
        private string? _platesNumber = "";
        private string? _area = "";
        private string? _constructionYear = "";
        private string? _price = "";
        private string? _deposit = "";
        private string? _rentAmount = "";
        private string? _payment = "";
        private string? _address = "";
        private string? _description = "";
        private string? _time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        private string? _date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        private string? _type = "";
        private bool _water = true;
        private bool _electricity = true;
        private bool _gas = true;
        private bool _phone = true;
        private readonly ICollection<AssetImage> _assetImages = [];
        private readonly ICollection<Person> _persons = [];

        [Key]
        public Guid Id
        {
            get => _id;
            set => _id = value;
        }

        [Required]
        public string? Name
        {
            get => _name;
            set => _name = value;
        }

        [Required]
        public string? PlatesNumber
        {
            get => _platesNumber;
            set => _platesNumber = value;
        }

        [Required]
        public string? Area
        {
            get => _area;
            set => _area = value;
        }

        [Required]
        public string? ConstructionYear
        {
            get => _constructionYear;
            set => _constructionYear = value;
        }

        [Required]
        public string? Price
        {
            get => _price;
            set => _price = value;
        }

        [Required]
        public string? Deposit
        {
            get => _deposit;
            set => _deposit = value;
        }

        [Required]
        public string? RentAmount
        {
            get => _rentAmount;
            set => _rentAmount = value;
        }

        [Required]
        public string? Payment
        {
            get => _payment;
            set => _payment = value;
        }

        [Required]
        public string? Address
        {
            get => _address;
            set => _address = value;
        }
        public string? Description
        {
            get => _description;
            set => _description = value;
        }
        public string? Time
        {
            get => _time;
            set => _time = value;
        }
        public string? Date
        {
            get => _date;
            set => _date = value;
        }
        [Required]
        public string? Type
        {
            get => _type;
            set => _type = value;
        }

        public bool Water
        {
            get => _water;
            set => _water = value;
        }
        public bool Electricity
        {
            get => _electricity;
            set => _electricity = value;
        }
        public bool Gas
        {
            get => _gas;
            set => _gas = value;
        }
        public bool Phone
        {
            get => _phone;
            set => _phone = value;
        }
        public ICollection<AssetImage> AssetImages => _assetImages;

        public ICollection<Person> Persons => _persons;
    }
}

