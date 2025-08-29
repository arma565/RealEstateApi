using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

#pragma warning disable CA1515
namespace RealEstate.Models.Estate.Assets
{
    public sealed class Asset
    {
        public Asset() { }

        private Guid _id;
        private int _orderID;
        private string? _assetType = "";
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
        private string? _contractType = "";
        private bool _water;
        private bool _electricity;
        private bool _gas;
        private bool _phone;
        private readonly ICollection<AssetImage> _assetImages = [];
        private readonly ICollection<Person> _persons = [];

        [Key]
        public Guid Id
        {
            get => _id;
            set => _id = value;
        }

        [DefaultValue(0)]
        public int OrderID
        {
            get => _orderID;
            set => _orderID = value;
        }

        [Required(ErrorMessage = "Asset name is required!")]
        public string? AssetType
        {
            get => _assetType;
            set => _assetType = value;
        }

        [Required(ErrorMessage = "PlatesNumber is required!")]
        public string? PlatesNumber
        {
            get => _platesNumber;
            set => _platesNumber = value;
        }

        [Required(ErrorMessage = "Area is required!")]
        public string? Area
        {
            get => _area;
            set => _area = value;
        }

        [Required(ErrorMessage = "ConstructionYear is required!")]
        public string? ConstructionYear
        {
            get => _constructionYear;
            set => _constructionYear = value;
        }

        [Required(ErrorMessage = "Price is required!")]
        public string? Price
        {
            get => _price;
            set => _price = value;
        }

        public string? Deposit
        {
            get => _deposit;
            set => _deposit = value;
        }

        public string? RentAmount
        {
            get => _rentAmount;
            set => _rentAmount = value;
        }

        [Required(ErrorMessage = "Payment is required!")]
        public string? Payment
        {
            get => _payment;
            set => _payment = value;
        }

        [Required(ErrorMessage = "Address is required!")]
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
        [Required(ErrorMessage = "ContractType is required!")]
        public string? ContractType
        {
            get => _contractType;
            set => _contractType = value;
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

