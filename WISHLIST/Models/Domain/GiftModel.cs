using System.ComponentModel.DataAnnotations.Schema;

namespace WISHLIST.Models.Domain
{
    public class GiftModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageFilePath { get; set; }

        public string GiftUrl { get; set; }

        public int Priority { get; set; }

        [Column(TypeName = "bit")]
        public bool IsFullfilled { get; set; }

        public int ModificatorId { get; set; }

        public string WishlistId { get; set; }

        [Column(TypeName ="date")]
        public DateTime LastUpdateDate { get; set; }

        public WishlistModel Wishlist { get; set; }

        public ModificatorModel Modificator { get; set; }
    }
}
