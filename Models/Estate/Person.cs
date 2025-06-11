using RealEstate.Models.Estate.Assets;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Estate
{
    public sealed class Person
    {
        public Person() { }

        public Person(
            string name,
            string fatherName,
            long birthCertificateNumber,
            string birthCertificateIssued,
            long personID,
            string born,
            string address,
            string phone,
            string role,
            Guid assetID
        )
        {
            _name = name;
            _fatherName = fatherName;
            _birthCertificateNumber = birthCertificateNumber;
            _birthCertificateIssued = birthCertificateIssued;
            _personID = personID;
            _born = born;
            _address = address;
            _phone = phone;
            _role = role;
            _assetID = assetID;
        }

        private Guid _id;
        private string _name = "";
        private string _fatherName = "";
        private long _birthCertificateNumber;
        private string _birthCertificateIssued = "";
        private long _personID;
        private string _born = "";
        private string _address = "";
        private string _phone = "";
        private string _role = "";
        private Asset? _asset;
        private Guid _assetID;

        [Key]
        public Guid Id
        {
            get => _id;
            set => _id = value;
        }

        [Required]
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        public string FatherName
        {
            get => _fatherName;
            set => _fatherName = value;
        }

        [Required]
        public long BirthCertificateNumber
        {
            get => _birthCertificateNumber;
            set => _birthCertificateNumber = value;
        }
        public string BirthCertificateIssued
        {
            get => _birthCertificateIssued;
            set => _birthCertificateIssued = value;
        }

        [Required]
        public long PersonID
        {
            get => _personID;
            set => _personID = value;
        }
        public string Born
        {
            get => _born;
            set => _born = value;
        }

        [Required]
        public string Address
        {
            get => _address;
            set => _address = value;
        }

        [Required]
        public string Phone
        {
            get => _phone;
            set => _phone = value;
        }
        public string Role
        {
            get => _role;
            set => _role = value;
        }
        public Guid AssetID
        {
            get => _assetID;
            set => _assetID = value;
        }
        public Asset? Asset
        {
            get => _asset;
            set => _asset = value;
        }
    }
}

