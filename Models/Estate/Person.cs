using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Estate
{
    public class Person
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
            Guid propertyID
        )
        {
            this.name = name;
            this.fatherName = fatherName;
            this.birthCertificateNumber = birthCertificateNumber;
            this.birthCertificateIssued = birthCertificateIssued;
            this.personID = personID;
            this.born = born;
            this.address = address;
            this.phone = phone;
            this.role = role;
            this.propertyID = propertyID;
        }

        private Guid id;
        private string name = "";
        private string fatherName = "";
        private long birthCertificateNumber;
        private string birthCertificateIssued = "";
        private long personID;
        private string born = "";
        private string address = "";
        private string phone = "";
        private string role = "";
        private Estate? property;
        private Guid propertyID;

        public Guid Id
        {
            get => id;
            set => id = value;
        }

        [Required]
        public string Name
        {
            get => name;
            set => name = value;
        }
        public string FatherName
        {
            get => fatherName;
            set => fatherName = value;
        }

        [Required]
        public long BirthCertificateNumber
        {
            get => birthCertificateNumber;
            set => birthCertificateNumber = value;
        }
        public string BirthCertificateIssued
        {
            get => birthCertificateIssued;
            set => birthCertificateIssued = value;
        }

        [Required]
        public long PersonID
        {
            get => personID;
            set => personID = value;
        }
        public string Born
        {
            get => born;
            set => born = value;
        }

        [Required]
        public string Address
        {
            get => address;
            set => address = value;
        }

        [Required]
        public string Phone
        {
            get => phone;
            set => phone = value;
        }
        public string Role
        {
            get => role;
            set => role = value;
        }
        public Guid PropertyID
        {
            get => propertyID;
            set => propertyID = value;
        }
        public Estate? Property
        {
            get => property;
            set => property = value;
        }
    }
}

