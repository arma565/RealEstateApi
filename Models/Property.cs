public class Property
{
    public Property() { }

    public Property(string? name,
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
            ICollection<Person> persons)
    {
        this.name = name;
        this.platesNumber = platesNumber;
        this.area = area;
        this.constructionYear = constructionYear;
        this.price = price;
        this.deposit = deposit;
        this.rentAmount = rentAmount;
        this.payment = payment;
        this.address = address;
        this.description = description;
        this.time = time;
        this.date = date;
        this.type = type;
        this.water = water;
        this.electricity = electricity;
        this.gas = gas;
        this.phone = phone;
        this.persons = persons;
    }

    private int id = 0;
    private string? name = "";
    private string? platesNumber = "";
    private string? area = "";
    private string? constructionYear = "";
    private string? price = "";
    private string? deposit = "";
    private string? rentAmount = "";
    private string? payment = "";
    private string? address = "";
    private string? description = "";
    private string? time = "";
    private string? date = "";
    private string? type = "";
    private bool water = true;
    private bool electricity = true;
    private bool gas = true;
    private bool phone = true;
    private readonly ICollection<Person> persons = new List<Person>();

    public int Id { get => id; set => id = value; }
    public string? Name { get => name; set => name = value; }
    public string? PlatesNumber { get => platesNumber; set => platesNumber = value; }
    public string? Area { get => area; set => area = value; }
    public string? ConstructionYear { get => constructionYear; set => constructionYear = value; }
    public string? Price { get => price; set => price = value; }
    public string? Deposit { get => deposit; set => deposit = value; }
    public string? RentAmount { get => rentAmount; set => rentAmount = value; }
    public string? Payment { get => payment; set => payment = value; }
    public string? Address { get => address; set => address = value; }
    public string? Description { get => description; set => description = value; }
    public string? Time { get => time; set => time = value; }
    public string? Date { get => date; set => date = value; }
    public string? Type { get => type; set => type = value; }
    public bool Water { get => water; set => water = value; }
    public bool Electricity { get => electricity; set => electricity = value; }
    public bool Gas { get => gas; set => gas = value; }
    public bool Phone { get => phone; set => phone = value; }
    public ICollection<Person> Persons => persons;

}
