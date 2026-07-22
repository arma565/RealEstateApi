using System.ComponentModel.DataAnnotations;

#pragma warning disable CA1515
#pragma warning disable CS0649
namespace RealEstate.Models.Assets;

public sealed class AssetImage
{
    public AssetImage() { }

    private Guid _id;
    private string _fileName = "";
    private Guid _assetID;
    private readonly Asset? _asset;

    [Key]
    public Guid Id
    {
        get => _id;
        set => _id = value;
    }

    [Required]
    public string FileName
    {
        get => _fileName;
        set => _fileName = value;
    }

    public Guid AssetID
    {
        get => _assetID;
        set => _assetID = value;
    }

    public Asset? Asset => _asset;

}
