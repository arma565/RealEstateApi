#pragma warning disable CA1515
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Support;

public sealed class SupportApp
{
    private Guid _id;
    private string? _title = "";
    private string? _details_title = "";
    private string? _details_subtitle = "";
    private IEnumerable<string> _details_description_list = [];
    private SupportImage? _supportImage;

    [Key]
    public Guid Id {
        get => _id; 
        set => _id = value;
    }
    public string? Title { 
        get => _title; 
        set => _title = value;
    }

    public string? DetailsTitle { 
        get => _details_title; 
        set => _details_title = value;
    }
    public string? DetailsSubtitle { 
        get => _details_subtitle; 
        set => _details_subtitle = value;
    }
    public IEnumerable<string> DetailsDescriptionList {
        get => _details_description_list;
        set => _details_description_list = value;
    }
    public SupportImage? SupportImage {
        get => _supportImage;
        set => _supportImage = value;
    }

}
