using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Estate.Assets
{
    public sealed class AssetImage
    {
        public AssetImage() { }

        public AssetImage(string fileName , Guid assetID) {
            _fileName = fileName;
            _assetID = assetID;
        }


        private Guid _id;
        private string _fileName;
        private Guid _assetID;
        private Asset? _asset;

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
        public Asset? Asset
        {
            get => _asset;
            set => _asset = value;
        }
        
    }
}
