using RealEstate.Models.Estate.Assets;
using System.ComponentModel.DataAnnotations;

#pragma warning disable CA1515
#pragma warning disable CS0649
namespace RealEstate.Models.Estate
{
    public sealed class Person
    {
        public Person() { }

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
        private Guid _assetID;
        private readonly Asset? _asset;
       
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
        public Asset? Asset => _asset;
    }
}

