using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Supports;

#pragma warning disable CA1515
public class CreateDTO
{
    [DefaultValue("")]
    [Required(ErrorMessage = "Title is required!")]
    public required string Title { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "DetailsTitle is required!")]
    public required string DetailsTitle { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "DetailsSubtitle is required!")]
    public required string DetailsSubtitle { get; set; } 

    public ICollection<string> DetailsDescriptionList => [];
}
