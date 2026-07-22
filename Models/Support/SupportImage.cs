
namespace RealEstate.Models.Support;

#pragma warning disable CA1515
public sealed class SupportImage
{
    private Guid _id;
    private string _supportImageFileName = "";
    private Guid supportId;
    private SupportApp? _support;

    public Guid Id {
        get => _id;
        set => _id = value; 
    }
    public string SupportImageFileName { 
        get => _supportImageFileName;
        set => _supportImageFileName = value;
    }
    public Guid SupportId {
        get => supportId;
        set => supportId = value;
    }
    public SupportApp? Support {
        get => _support; 
        set => _support = value;
    }
}
