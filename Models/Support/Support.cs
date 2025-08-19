#pragma warning disable CA1515
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Support
{
    public sealed class Support
    {
        private Guid _id;
        private string? title = "";
        private string? description = "";
        private SupportImage? _supportImage;

        [Key]
        public Guid Id {
            get => _id; 
            set => _id = value;
        }
        public string? Title { 
            get => title; 
            set => title = value;
        }
        public string? Description {
            get => description; 
            set => description = value;
        }
        public SupportImage? SupportImage {
            get => _supportImage;
            set => _supportImage = value;
        }
    }
}
