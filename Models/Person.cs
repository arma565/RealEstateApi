public class Person
{

    public Person()
    {
    }

    public Person(string name,
    string fatherName,
     long birthCertificateNumber,
      string birthCertificateIssued,
       long personID, string born,
        string address,
        string phone,
         string role,
         int propertyID)
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

    private int id = 0;
    private string name = "";
    private string fatherName = "";
    private long birthCertificateNumber = 0;
    private string birthCertificateIssued = "";
    private long personID = 0;
    private string born = "";
    private string address = "";
    private string phone = "";
    private string role = "";
    private Property? property;
    private int propertyID;

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string FatherName { get => fatherName; set => fatherName = value; }
    public long BirthCertificateNumber { get => birthCertificateNumber; set => birthCertificateNumber = value; }
    public string BirthCertificateIssued { get => birthCertificateIssued; set => birthCertificateIssued = value; }
    public long PersonID { get => personID; set => personID = value; }
    public string Born { get => born; set => born = value; }
    public string Address { get => address; set => address = value; }
    public string Phone { get => phone; set => phone = value; }
    public string Role { get => role; set => role = value; }
    public int PropertyID { get => propertyID; set => propertyID = value; }
    public Property? Property { get => property; set => property = value; }
}